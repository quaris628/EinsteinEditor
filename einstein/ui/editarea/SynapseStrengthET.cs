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
    public class SynapseStrengthET : FloatET
    {
        public static readonly Color TEXT_COLOR = EinsteinConfig.COLOR_MODE.Text;

        public BaseSynapse Synapse { get; protected set; }
        private bool justEnabledEditing;

        public SynapseStrengthET(BaseSynapse synapse, int anchorX, int anchorY)
            : base(new FloatETBuilder(new Text.TextBuilder("").WithColor(new SolidBrush(TEXT_COLOR)).Build())
                  .WithEditingDisabled()
                  .WithMinValue(BibiteVersionIndependentConfig.SYNAPSE_STRENGTH_MIN)
                  .WithMaxValue(BibiteVersionIndependentConfig.SYNAPSE_STRENGTH_MAX)
                  .WithMaxDecimalPlaces(BaseSynapse.STRENGTH_MAX_DECIMALS))
        {
            Synapse = synapse;
            justEnabledEditing = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetValue(Synapse.getStrengthAsStringForUI());
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
            UpdateStrengthIfValid();
        }

        public override void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateStrengthIfValid();
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
            UpdateStrengthIfValid();
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
        
        public override void RecenterOnAnchor()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            GetDrawable().SetCenterXY(anchorX, anchorY);
        }

        private void UpdateStrengthIfValid()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            try
            {
                Synapse.setStrengthAsStringForUI(text.GetMessage());
            }
            catch (ArgumentException)
            {
                // Strength can be left as its most recent valid value
            }
            catch (ArithmeticException)
            {
                // Strength can be left as its most recent valid value
            }
        }
    }
}
