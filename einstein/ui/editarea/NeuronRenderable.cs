using Einstein.config;
using Einstein.model;
using Einstein.ui.menu;
using LibraryFunctionReplacements;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class NeuronRenderable : Draggable
    {
        public const int SPAWN_X = 250 + MenuCategoryButton.WIDTH + EinsteinConfig.PAD;
        public const int SPAWN_Y = 200 + (MenuCategoryButton.HEIGHT + EinsteinConfig.PAD) * 3;

        public const int BIAS_TEXT_LAYER = 10;
        public const int BIAS_X_OFFSET = -10;
        public const int BIAS_Y_OFFSET = -10;

        public BaseNeuron Neuron { get; private set; }
        public NeuronDrawable NeuronDrawable { get { return (NeuronDrawable)GetDrawable(); } }
        private NeuronBiasET nbet; // also accessible via the text variable, but this is a nice shortcut reference
        private SelectableEditableText biasText;

        private EditArea editArea;
        private bool isRemoved;

        public NeuronRenderable(EditArea editArea, BaseNeuron neuron, bool tryPainting)
            : base(new NeuronDrawable(neuron, SPAWN_X, SPAWN_Y), EditArea.GetBounds)
        {
            Neuron = neuron;
            NeuronDrawable.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
            this.editArea = editArea;
            isRemoved = false;
            if (Neuron.BibiteVersion.HasBiases())
            {
                nbet = new NeuronBiasET(Neuron,
                NeuronDrawable.GetCircleCenterX() + BIAS_X_OFFSET,
                NeuronDrawable.GetCircleCenterY() + BIAS_Y_OFFSET);
                biasText = new SelectableEditableText(nbet,
                    CustomNumberParser.FloatToString(neuron.Bias),
                    EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                    EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);
            }

            if (tryPainting
                && (editArea.isPainting || IO.KEYS.IsModifierKeyDown(Keys.Control)))
            {
                NeuronDrawable.SetColorGroup(editArea.PaintColor);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            IO.RENDERER.Add(this);
            if (Neuron.BibiteVersion.HasBiases())
            {
                IO.RENDERER.Add(biasText, BIAS_TEXT_LAYER);
                biasText.Initialize();
                IO.MOUSE.LEFT_CLICK.SubscribeOnDrawable(MaybeRemoveOrPaint, biasText.GetDrawable());
                IO.MOUSE.RIGHT_UP.SubscribeOnDrawable(StartASynapse, biasText.GetDrawable());
            }

            IO.MOUSE.LEFT_CLICK.SubscribeOnDrawable(MaybeRemoveOrPaint, GetDrawable());
            IO.MOUSE.RIGHT_UP.SubscribeOnDrawable(StartASynapse, GetDrawable());
            if (Neuron.IsHidden())
            {
                NeuronDrawable.EnableEditingDescription(editArea.Brain);
            }
            Reposition();
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            IO.RENDERER.Remove(this);
            if (Neuron.BibiteVersion.HasBiases())
            {
                IO.RENDERER.Remove(biasText);
                biasText.Uninitialize();
                IO.MOUSE.LEFT_CLICK.UnsubscribeFromDrawable(MaybeRemoveOrPaint, biasText.GetDrawable());
                IO.MOUSE.RIGHT_UP.UnsubscribeFromDrawable(StartASynapse, biasText.GetDrawable());
            }

            IO.MOUSE.LEFT_CLICK.UnsubscribeFromDrawable(MaybeRemoveOrPaint, GetDrawable());
            IO.MOUSE.RIGHT_UP.UnsubscribeFromDrawable(StartASynapse, GetDrawable());
            if (Neuron.IsHidden())
            {
                NeuronDrawable.DisableEditingDescription();
            }
        }

        private void MaybeRemoveOrPaint()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }
            if (IO.KEYS.IsModifierKeyDown(Keys.Shift))
            {
                editArea.RemoveNeuron(Neuron);
                isRemoved = true;
            }
            else if (editArea.isPainting || IO.KEYS.IsModifierKeyDown(Keys.Control))
            {
                NeuronDrawable.SetColorGroup(editArea.PaintColor);
            }
        }

        private void StartASynapse(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }
            editArea.StartSynapse(this, x, y);
        }

        protected override void MyMouseDown(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }
            editArea.DisableShiftingView();

        }

        protected override void MyMouseMove(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }
            editArea.Brain.FlagChange();
            Reposition();
        }

        public void Reposition()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }
            Reposition(NeuronDrawable.GetCircleCenterX(), NeuronDrawable.GetCircleCenterY());
        }

        public void Reposition(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (isRemoved) { return; }

            if (NeuronDrawable.GetCircleCenterX() != x && NeuronDrawable.GetCircleCenterY() != y)
            {
                editArea.Brain.FlagChange();
            }
            NeuronDrawable.SetCircleCenterXY(x, y);
            UpdateBiasTextPosition();

            if (editArea.Brain != null)
            {
                foreach (BaseSynapse synapseFrom in editArea?.Brain?.GetSynapsesFrom(Neuron))
                {
                    editArea.GetSROf(synapseFrom).UpdateBasePositionToFromNeuron();
                }
                foreach (BaseSynapse synapseTo in editArea?.Brain?.GetSynapsesTo(Neuron))
                {
                    editArea.GetSROf(synapseTo).UpdateTipPositionToToNeuron();
                }
            }
        }

        private void UpdateBiasTextPosition()
        {
            if (Neuron.BibiteVersion.HasBiases())
            {
                nbet.SetAnchor(NeuronDrawable.GetCircleCenterX() + BIAS_X_OFFSET,
                    NeuronDrawable.GetCircleCenterY() + BIAS_Y_OFFSET);
            }
        }

        public override string ToString()
        {
            return "NR [" +
                "Neuron: " + Neuron +
                "NeuronDrawable: " + NeuronDrawable +
                "isInit: " + isInit +
                "isRemoved: " + isRemoved +
                "]";
        }
    }
}
