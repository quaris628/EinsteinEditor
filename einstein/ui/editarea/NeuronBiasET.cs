using Einstein.config;
using Einstein.model;
using LibraryFunctionReplacements;
using phi.graphics.drawables;
using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    // a modified copy of SynapseStrengthET
    public class NeuronBiasET : FloatET
    {
        public const int FONT_SIZE = 11;

        // Neuron (and its version) must be immutable (otherwise init/uninit is fucked up)
        public BaseNeuron Neuron { get; protected set; }
        private bool justEnabledEditing;

        public NeuronBiasET(BaseNeuron neuron, int anchorX, int anchorY)
            : base(
                (FloatETBuilder)new FloatETBuilder(
                    new Text.TextBuilder("")
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    .WithFontSize(FONT_SIZE)
                    .Build())
                .WithEditingDisabled()
                .WithMinValue(BibiteConfigVersionIndependent.NEURON_BIAS_MIN)
                .WithMaxValue(BibiteConfigVersionIndependent.NEURON_BIAS_MAX)
                .WithMaxDecimalPlaces(BibiteConfigVersionIndependent.NEURON_BIAS_MAX_DECIMALS)
                .WithAnchor(anchorX, anchorY)
                .WithAnchorPosition(AnchorPosition.BottomRight))
        {
            Neuron = neuron;
            justEnabledEditing = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetValue(Neuron.getBiasAsStringForUI());
        }

        public void SetValue(string strength)
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
        }
        public override void Backspace()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Backspace();
            UpdateBiasIfValid();
        }

        public override void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateBiasIfValid();
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
            UpdateBiasIfValid();
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
        
        private void UpdateBiasIfValid()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            try
            {
                Neuron.setBiasAsStringForUI(text.GetMessage());
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
