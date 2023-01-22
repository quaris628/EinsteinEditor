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
        private bool justEnabledEditing;

        public SynapseStrengthET(BaseSynapse synapse, Line line)
            : base(new FloatETBuilder(new Text(""))
                  .WithEditingDisabled()
                  .WithMinValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MIN)
                  .WithMaxValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MAX)
                  .WithMaxDecimalPlaces(SYNAPSE_STRENGTH_MAX_DECIMALS))
        {
            Synapse = synapse;
            this.line = line;
            justEnabledEditing = false;
            SetValue(synapse.Strength);
        }

        public void SetValue(float strength)
        {
            // easy to do this way b/c validation is all taken care of underneath
            // somewhat inefficient but whatever

            bool wasEnabled = IsEditingEnabled;
            if (!wasEnabled) { EnableEditing(); }
            Clear();
            string strStr = Math.Round(strength, 2).ToString(); // strength string
            foreach (char c in strStr)
            {
                TypeChar(c);
            }
            if (!wasEnabled) { DisableEditing(); }
        }
        public override void Backspace()
        {
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Backspace();
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public override void Clear()
        {
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public override void TypeChar(char c)
        {
            if (!IsEditingEnabled) { return; }
            if (justEnabledEditing)
            {
                justEnabledEditing = false;
                Clear();
            }
            base.TypeChar(c);
            UpdateStrengthIfValid();
            ReCenterOnLine();
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
