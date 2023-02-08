using Einstein.config;
using Einstein.model;
using Einstein.model.json;
using Einstein.ui.editarea.visibleElements;
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

/* removing a neuron doesn't remove any linked synapse MRs
(including synapses coming from the neuron but not linked to any To neuron yet)

*/

namespace Einstein.ui.editarea
{
    public class SynapseRenderable : LineArrow
    {
        public const int ARROW_LAYER = 5;
        public const int TEXT_LAYER = 6;
        
        public const float INITIAL_STRENGTH = 1f;
        public const string DEFAULT_STRENGTH = "0";

        public BaseSynapse Synapse { get; private set; }
        public NeuronRenderable From { get; private set; }
        public NeuronRenderable To { get; private set; }

        private EditArea editArea;
        private bool isFinalized;
        private SynapseStrengthET sset; // also in text, but kept as a shortcut reference
        private SelectableEditableText text;
        

        public SynapseRenderable(EditArea editArea, BaseSynapse synapse,
            NeuronRenderable from, NeuronRenderable to)
            : base(from.NeuronDrawable.GetCircleCenterX(),
                  from.NeuronDrawable.GetCircleCenterY(),
                  to.NeuronDrawable.GetCircleCenterX(),
                  to.NeuronDrawable.GetCircleCenterY())
        {
            this.editArea = editArea;
            Synapse = synapse;
            From = from;
            To = to;
            isFinalized = true;
            sset = new SynapseStrengthET(Synapse, line);
            text = new SelectableEditableText(sset, DEFAULT_STRENGTH,
                EinsteinPhiConfig.COLOR_MODE.SynapseTextBackgroundSelected,
                EinsteinPhiConfig.COLOR_MODE.SynapseTextBackgroundUnselected);
        }

        public SynapseRenderable(EditArea editArea, NeuronRenderable from, int mouseX, int mouseY)
            : base(from.NeuronDrawable.GetCircleCenterX(),
                  from.NeuronDrawable.GetCircleCenterY(),
                  mouseX, mouseY)
        {
            this.editArea = editArea;
            Synapse = null;
            From = from;
            To = null;
            isFinalized = false;
            text = null;
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
                || toNeuronR.Neuron.Equals(From.Neuron)
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

            sset = new SynapseStrengthET(Synapse, line);
            text = new SelectableEditableText(sset, DEFAULT_STRENGTH,
                EinsteinPhiConfig.COLOR_MODE.SynapseTextBackgroundSelected,
                EinsteinPhiConfig.COLOR_MODE.SynapseTextBackgroundUnselected);
            IO.RENDERER.Add(text, TEXT_LAYER);
            text.Initialize();

            IO.MOUSE.MOVE.Unsubscribe(UpdateTipXY);
            IO.MOUSE.RIGHT_UP.Unsubscribe(TryFinalize);
            IO.MOUSE.LEFT_UP.Subscribe(RemoveIfShiftDownAndExactlyContainsClick);
            IO.MOUSE.LEFT_UP.SubscribeOnDrawable(RemoveIfShiftDown, text.GetDrawable());

            UpdateTipPositionToToNeuron();
            editArea.FinishSynapse(Synapse);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            if (isFinalized)
            {
                IO.RENDERER.Remove(text);
                text.Uninitialize();
                IO.MOUSE.LEFT_UP.Unsubscribe(RemoveIfShiftDownAndExactlyContainsClick);
                IO.MOUSE.LEFT_UP.UnsubscribeFromDrawable(RemoveIfShiftDown, text.GetDrawable());
            }
            else
            {
                IO.MOUSE.MOVE.Unsubscribe(UpdateTipXY);
                IO.MOUSE.RIGHT_UP.Unsubscribe(TryFinalize);
            }
        }

        // ----- Updating positions -----

        public void UpdateBasePositionToFromNeuron()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
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
            sset.ReCenterOnLine();
        }

        // ----- Removing -----

        private void RemoveIfShiftDown()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (IO.KEYS.IsModifierKeyDown(Keys.Shift))
            {
                editArea.RemoveSynapse(Synapse);
            }
        }

        private void RemoveIfShiftDownAndExactlyContainsClick(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            // We only want to remove the synapse if the click was on this line,
            // i.e., if the distance is less than half of the line's width,
            // or if the click is inside the triangle of the arrow

            if (IO.KEYS.IsModifierKeyDown(Keys.Shift)
                && line.GetBoundaryRectangle().Contains(x, y)
                && (line.CalcSqDistanceToLine(x, y) <= HALF_LINE_WIDTH * HALF_LINE_WIDTH
                || arrow.TriangleContainsPoint(x, y)))
            {
                editArea.RemoveSynapse(Synapse);
            }
        }

        // ----- overrides -----

        public override string ToString()
        {
            return string.Format(
                "SynapseRenderable: Synapse = [{0}] From = [{1}] To = [{2}]",
                Synapse.ToString(), From.ToString(), To.ToString());
        }
    }
}
