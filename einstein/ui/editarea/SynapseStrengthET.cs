using Einstein.model;
using Einstein.ui.editarea.visibleElements;
using phi.graphics.drawables;
using phi.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class SynapseStrengthET : FloatET
    {
        public const int SYNAPSE_STRENGTH_MAX_DECIMALS = 2;

        public BaseSynapse Synapse { get; protected set; }
        private Line line;

        public SynapseStrengthET(BaseSynapse synapse, Line line)
            : base(new FloatETBuilder(new Text(synapse.Strength.ToString()))
                  .WithEditingDisabled()
                  .WithMinValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MIN)
                  .WithMaxValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MAX)
                  .WithMaxDecimalPlaces(SYNAPSE_STRENGTH_MAX_DECIMALS))
        {
            Synapse = synapse;
            this.line = line;
        }

        public void SetValue(float strength)
        {
            // easy to do this way b/c validation is all taken care of underneath
            // somewhat inefficient but whatever
            bool wasEnabled = IsEditingEnabled;
            EnableEditing();
            Clear();
            foreach (char c in strength.ToString())
            {
                TypeChar(c);
            }
            if (!wasEnabled) { DisableEditing(); }
        }
        public override void Backspace()
        {
            if (!IsEditingEnabled) { return; }
            base.Backspace();
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public override void Clear()
        {
            if (!IsEditingEnabled) { return; }
            base.Clear();
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public override void TypeChar(char c)
        {
            if (!IsEditingEnabled) { return; }
            base.TypeChar(c);
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public void ReCenterOnLine()
        {
            GetDrawable().SetCenterXY(line.GetCenterX(), line.GetCenterY());
        }

        private void UpdateStrengthIfValid()
        {
            if (float.TryParse(text.GetMessage(), out float strength))
            {
                Synapse.Strength = strength;
            }
        }
    }
}
