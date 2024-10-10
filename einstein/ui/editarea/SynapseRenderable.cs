using Einstein.config;
using Einstein.model;
using Einstein.model.json;
using phi.graphics;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class SynapseRenderable : LineArrow
    {
        public const int ARROW_LAYER = 5;
        public const int TEXT_LAYER = 7;
        
        public const float INITIAL_STRENGTH = 1f;
        public const string DEFAULT_STRENGTH = "0";

        private const float STRENGTH_POS_ALONG_SYNAPSE_LENGTH = 0.7f;

        private const string CIRCLE_ARROW_IMAGE = EinsteinConfig.RES_DIR + "CircularSynapse.png";
        private const string CIRCLE_ARROW_DOTTED_IMAGE = EinsteinConfig.RES_DIR + "CircularSynapseDotted.png";

        public BaseSynapse Synapse { get; private set; }
        public NeuronRenderable From { get; private set; }
        public NeuronRenderable To { get; private set; }

        private EditArea editArea;
        private bool isFinalized;
        private SynapseStrengthET sset; // also accessible via the text variable, but this is a nice shortcut reference
        private SelectableEditableText strengthText;
        private Sprite circleArrow;


        public SynapseRenderable(EditArea editArea, BaseSynapse synapse,
            NeuronRenderable from, NeuronRenderable to)
            : base(from.NeuronDrawable.GetCircleCenterX(),
                  from.NeuronDrawable.GetCircleCenterY(),
                  to.NeuronDrawable.GetCircleCenterX(),
                  to.NeuronDrawable.GetCircleCenterY(),
                  synapse.IsEnabled)
        {
            this.editArea = editArea;
            Synapse = synapse;
            From = from;
            To = to;
            isFinalized = true;
            sset = new SynapseStrengthET(Synapse, line.GetCenterX(), line.GetCenterY());
            strengthText = new SelectableEditableText(sset, DEFAULT_STRENGTH,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);
        }

        public SynapseRenderable(EditArea editArea, NeuronRenderable from, int mouseX, int mouseY)
            : base(from.NeuronDrawable.GetCircleCenterX(),
                  from.NeuronDrawable.GetCircleCenterY(),
                  mouseX, mouseY, true)
        {
            this.editArea = editArea;
            Synapse = null;
            From = from;
            To = null;
            isFinalized = false;
            strengthText = null;
        }

        // ----- Initialize, Finalize, and Uninitialize -----

        private new void Initialize(int layer) { } // hide parent method
        public void Initialize()
        {
            base.Initialize(ARROW_LAYER);
            IO.MOUSE.MOVE.Subscribe(UpdateTipXY);
            IO.MOUSE.RIGHT_UP.Subscribe(TryFinalize);
            if (isFinalized)
            {
                FinalizeInternal();
            }
        }

        private void TryFinalize(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            NeuronRenderable toNeuronR = editArea.HasNeuronAtPosition(x, y);
            if (toNeuronR == null
                || (From == toNeuronR && From.Neuron.IsInput())
                || !editArea.Brain.ContainsNeuron(toNeuronR.Neuron)
                || editArea.Brain.ContainsSynapse(From.Neuron.Index, toNeuronR.Neuron.Index))
            {
                editArea.ClearStartedSynapse(toNeuronR != null);
            }
            else
            {
                Finalize(toNeuronR);
            }
        }

        public void Finalize(NeuronRenderable to)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            To = to;
            Synapse = new JsonSynapse((JsonNeuron)From.Neuron,
                (JsonNeuron)To.Neuron, INITIAL_STRENGTH);
            FinalizeInternal();
        }

        private void FinalizeInternal()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            isFinalized = true;

            sset = new SynapseStrengthET(Synapse, line.GetCenterX(), line.GetCenterY());
            strengthText = new SelectableEditableText(sset, DEFAULT_STRENGTH,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);
            IO.RENDERER.Add(strengthText, TEXT_LAYER);
            strengthText.Initialize();

            IO.MOUSE.MOVE.Unsubscribe(UpdateTipXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(TryFinalize);
            IO.MOUSE.LEFT_UP.Subscribe(OnClickArrowRectangle);
            IO.MOUSE.LEFT_UP.SubscribeOnDrawable(OnClickStrengthText, strengthText.GetDrawable());

            if (From == To)
            {
                IO.RENDERER.Remove(line);
                IO.RENDERER.Remove(arrow);
                circleArrow = new Sprite(
                    new ImageWrapper(Synapse.IsEnabled ? CIRCLE_ARROW_IMAGE : CIRCLE_ARROW_DOTTED_IMAGE),
                    line.GetCenterX(), line.GetCenterY());
                IO.RENDERER.Add(circleArrow, ARROW_LAYER);
            }

            SetColor(To.Neuron.ColorGroup);
            UpdateTipPositionToToNeuron();
            editArea.FinishSynapse(Synapse);
        }

        public override void Uninitialize()
        {
            if (isFinalized)
            {
                if (From == To)
                {
                    if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
                    isInit = false;
                    IO.RENDERER.Remove(circleArrow);
                }
                else
                {
                    base.Uninitialize();
                }

                IO.RENDERER.Remove(strengthText);
                strengthText.Uninitialize();
                IO.MOUSE.LEFT_UP.Unsubscribe(OnClickArrowRectangle);
                IO.MOUSE.LEFT_UP.UnsubscribeFromDrawable(OnClickStrengthText, strengthText.GetDrawable());
            }
            else
            {
                base.Uninitialize();
                IO.MOUSE.MOVE.Unsubscribe(UpdateTipXY);
                IO.MOUSE.RIGHT_UP.Unsubscribe(TryFinalize);
            }
        }

        // ----- Updating positions -----

        public void UpdateBasePositionToFromNeuron()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            if (From == To)
            {
                UpdateCircleArrowPositionToNeuron();
                return;
            }

            int x = From.NeuronDrawable.GetCircleCenterX();
            int y = From.NeuronDrawable.GetCircleCenterY();
            UpdateBaseXY(x, y);

            if (isFinalized)
            {
                UpdateTipPositionToToNeuron();
            }
        }
        public void UpdateTipPositionToToNeuron()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!isFinalized) { throw new InvalidOperationException("Finalize before running UpdateTipPOsitionToToNeuron"); }

            if (From == To)
            {
                UpdateCircleArrowPositionToNeuron();
                return;
            }

            // TODO maybe check if this method implementation is right
            int circleCenterX = To.NeuronDrawable.GetCircleCenterX();
            int circleCenterY = To.NeuronDrawable.GetCircleCenterY();
            float slopeDeltaX = circleCenterX - From.NeuronDrawable.GetCircleCenterX();
            float slopeDeltaY = circleCenterY - From.NeuronDrawable.GetCircleCenterY();
            
            int arrowTipX = circleCenterX;
            int arrowTipY = circleCenterY;
            // if neurons are not exactly on top of each other
            if (slopeDeltaX != 0 || slopeDeltaY != 0)
            {
                float inputSlopeLength = (float)Math.Sqrt(
                slopeDeltaX * slopeDeltaX + slopeDeltaY * slopeDeltaY);
                float dX = -slopeDeltaX / inputSlopeLength;
                float dY = -slopeDeltaY / inputSlopeLength;
                arrowTipX = (int)(circleCenterX + dX * NeuronDrawable.CIRCLE_RADIUS);
                arrowTipY = (int)(circleCenterY + dY * NeuronDrawable.CIRCLE_RADIUS);
            }
            
            UpdateTipXY(arrowTipX, arrowTipY);

            UpdateStrengthTextPosition();
        }

        public void UpdateCircleArrowPositionToNeuron()
        {
            // neuron's circle center is at 43, 45 in the png
            int arrowUpperLeftX = To.NeuronDrawable.GetCircleCenterX() - 43;
            int arrowUpperLeftY = To.NeuronDrawable.GetCircleCenterY() - 45;
            circleArrow.SetXY(arrowUpperLeftX, arrowUpperLeftY);
            // text's anchor is at 9, 9 in the png
            sset.SetAnchor(arrowUpperLeftX + 9, arrowUpperLeftY + 9);
        }

        private void UpdateStrengthTextPosition()
        {
            float textX = line.GetX() + line.GetWidth() * STRENGTH_POS_ALONG_SYNAPSE_LENGTH;
            float textY = line.GetY() + line.GetHeight() * STRENGTH_POS_ALONG_SYNAPSE_LENGTH;
            sset.SetAnchor((int)textX, (int)textY);
        }

        // ----- On-Click -----

        private void OnClickArrowRectangle(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            // We only want to respond to clicks if the click was on the arrow's shape,
            if (From == To && circleArrow.GetBoundaryRectangle().Contains(x, y))
            {
                // i.e., for circular arrowss, is it inside the circle the arrow makes?

                // neuron's circle center is at 43, 45 in the png
                // arrow circle's center is at 22, 22 in the png
                int arrowCircleCenterX = To.NeuronDrawable.GetCircleCenterX() - 43 + 22;
                int arrowCircleCenterY = To.NeuronDrawable.GetCircleCenterY() - 45 + 22;
                int dx = x - arrowCircleCenterX;
                int dy = y - arrowCircleCenterY;
                if (dx * dx + dy * dy <= 22 * 22)
                {
                    OnClickArrow();
                }
            }
            // i.e. for straight arrows, if the distance is less than half of the line's width,
            // or if the click is inside the triangle of the arrow
            else if ((line.CalcSqDistanceToLine(x, y) <= HALF_LINE_WIDTH * HALF_LINE_WIDTH
                && (line.GetX() <= x && x <= line.GetX2() || line.GetX2() <= x && x <= line.GetX()
                    || line.GetY() <= y && y <= line.GetY2() || line.GetY2() <= y && y <= line.GetY()))
                || arrow.TriangleContainsPoint(x, y))
            {
                OnClickArrow();
            }
        }

        private void OnClickArrow()
        {
            if (IO.KEYS.IsModifierKeyDown(Keys.Shift))
            {
                editArea.RemoveSynapse(Synapse);
            }
            else if (IO.KEYS.IsModifierKeyDown(Keys.Alt))
            {
                Synapse.IsEnabled = !Synapse.IsEnabled;
                if (From == To)
                {
                    IO.RENDERER.Remove(circleArrow);
                    circleArrow = new Sprite(
                        new ImageWrapper(Synapse.IsEnabled ? CIRCLE_ARROW_IMAGE : CIRCLE_ARROW_DOTTED_IMAGE),
                        circleArrow.GetX(), circleArrow.GetY());
                    IO.RENDERER.Add(circleArrow, ARROW_LAYER);
                }
                else
                {
                    SetIsSolid(Synapse.IsEnabled);
                }
            }
        }

        private void OnClickStrengthText()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (IO.KEYS.IsModifierKeyDown(Keys.Shift))
            {
                editArea.RemoveSynapse(Synapse);
            }
            else if (IO.KEYS.IsModifierKeyDown(Keys.Alt))
            {
                Synapse.IsEnabled = !Synapse.IsEnabled;
                SetIsSolid(Synapse.IsEnabled);
                IO.FRAME_TIMER.QueueUninit(() => { sset.DisableEditing(); }); // kinda hacky but whatever
            }
        }

        // ----- overrides -----

        public override IEnumerable<Drawable> GetDrawables()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (From == To)
            {
                yield return circleArrow;
            }
            else
            {
                yield return line;
                yield return arrow;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "SynapseRenderable: Synapse = [{0}] From = [{1}] To = [{2}]",
                Synapse.ToString(), From.ToString(), To.ToString());
        }
    }
}
