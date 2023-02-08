using Einstein.config;
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
        public const int MAX_DECIMALS = 2;
        public static readonly Color TEXT_COLOR = EinsteinConfig.COLOR_MODE.Text;

        public BaseSynapse Synapse { get; protected set; }
        private Line line;
        private bool justEnabledEditing;

        public SynapseStrengthET(BaseSynapse synapse, Line line)
            : base(new FloatETBuilder(new Text.TextBuilder("").WithColor(new SolidBrush(TEXT_COLOR)).Build())
                  .WithEditingDisabled()
                  .WithMinValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MIN)
                  .WithMaxValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MAX)
                  .WithMaxDecimalPlaces(MAX_DECIMALS))
        {
            Synapse = synapse;
            this.line = line;
            justEnabledEditing = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetValue(Synapse.Strength);
        }

        public void SetValue(float strength)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
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
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Backspace();
            UpdateStrengthIfValid();
            ReCenterOnLine();
        }

        public override void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateStrengthIfValid();
            ReCenterOnLine();
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
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            GetDrawable().SetCenterXY(line.GetCenterX(), line.GetCenterY());
        }

        private void UpdateStrengthIfValid()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (float.TryParse(text.GetMessage(), out float strength))
            {
                Synapse.Strength = strength;
            }
        }
    }
}
