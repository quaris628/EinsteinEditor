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

namespace Einstein.ui.editarea
{
    public class SynapseStrengthET : FloatET
    {
        public const int MAX_DECIMALS = 2;
        public static readonly Color TEXT_COLOR = EinsteinConfig.COLOR_MODE.Text;

        public BaseSynapse Synapse { get; protected set; }
        private int anchorX;
        private int anchorY;
        private bool justEnabledEditing;

        public SynapseStrengthET(BaseSynapse synapse, int anchorX, int anchorY)
            : base(new FloatETBuilder(new Text.TextBuilder("").WithColor(new SolidBrush(TEXT_COLOR)).Build())
                  .WithEditingDisabled()
                  .WithMinValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MIN)
                  .WithMaxValue(BibiteVersionConfig.SYNAPSE_STRENGTH_MAX)
                  .WithMaxDecimalPlaces(MAX_DECIMALS))
        {
            Synapse = synapse;
            this.anchorX = anchorX;
            this.anchorY = anchorY;
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
            RecenterOnAnchor();
        }

        public override void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            justEnabledEditing = false;
            base.Clear();
            UpdateStrengthIfValid();
            RecenterOnAnchor();
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
            RecenterOnAnchor();
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
            RecenterOnAnchor();
        }

        public void SetAnchor(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            anchorX = x;
            anchorY = y;
            RecenterOnAnchor();
        }

        public void RecenterOnAnchor()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            GetDrawable().SetCenterXY(anchorX, anchorY);
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
