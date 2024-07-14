using Einstein.config;
using Einstein.model;
using phi.graphics.drawables;
using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static phi.graphics.renderables.EditableText;
using static phi.graphics.renderables.FloatET;

namespace Einstein.ui.editarea
{
    public class NeuronValueET : FloatET
    {
        public const int FONT_SIZE = 11;

        // Neuron (and its version) must be immutable (otherwise init/uninit is fucked up)
        public BaseNeuron Neuron { get; protected set; }
        private bool justEnabledEditing;

        public NeuronValueET(BaseNeuron neuron, int anchorX, int anchorY)
            : base(
                (FloatETBuilder)new FloatETBuilder(
                    new Text.TextBuilder("")
                    // colors are intentionally reversed
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected))
                    .WithBackgroundColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    .WithFontSize(FONT_SIZE)
                    .Build())
                .WithEditingDisabled()
                .WithMinValue(BibiteConfigVersionIndependent.NEURON_VALUE_MIN)
                .WithMaxValue(BibiteConfigVersionIndependent.NEURON_VALUE_MAX)
                .WithMaxDecimalPlaces(BibiteConfigVersionIndependent.NEURON_VALUE_MAX_DECIMALS)
                .WithAnchor(anchorX, anchorY)
                .WithAnchorPosition(AnchorPosition.CenterLeft))
        {
            Neuron = neuron;
            justEnabledEditing = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetValue(Neuron.getValueAsStringForUI(), Neuron.Value);
        }

        public void SetValue(string strength, float originalValue)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            // easy to do this way b/c validation is all taken care of underneath
            // somewhat inefficient but whatever

            bool wasEnabled = IsEditingEnabled;
            if (!wasEnabled) { EnableEditing(); }
            Clear();
            foreach (char c in strength)
            {
                TypeChar(c);
            }
            if (!wasEnabled) { DisableEditing(); }
            Neuron.Value = originalValue;
        }
        public override void Backspace()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Backspace();
            UpdateValueIfValid();
        }

        public override void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateValueIfValid();
        }

        public override void TypeChar(char c)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            if (justEnabledEditing)
            {
                justEnabledEditing = false;
                Clear();
            }
            base.TypeChar(c);
            UpdateValueIfValid();
        }

        public override void EnableEditing()
        {
            base.EnableEditing();
            justEnabledEditing = true;
        }

        public override void DisableEditing()
        {
            justEnabledEditing = false;
            base.DisableEditing();
        }

        private void UpdateValueIfValid()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            try
            {
                Neuron.setValueAsStringForUI(text.GetMessage());
            }
            catch (ArgumentException)
            {
                // Bias can be left as its most recent valid value
            }
            catch (ArithmeticException)
            {
                // Bias can be left as its most recent valid value
            }
        }
    }
}
