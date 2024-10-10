using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config
{
    public class ColorScheme
    {
        public enum Mode
        {
            Light,
            Dark,
        }

        public readonly Mode ColorMode;
        public readonly Color Background;
        public readonly Color MenuBackground;
        public readonly Color Text;
        public readonly Color DisabledButtonText;
        public readonly Color SynapseBase;
        public readonly Color SynapseTip;
        public readonly Color EditableTextBackgroundSelected;
        public readonly Color EditableTextBackgroundUnselected;

        public ColorScheme(Mode mode)
        {
            ColorMode = mode;
            if (mode == Mode.Light)
            {
                Background = Color.White;
                MenuBackground = Color.FromArgb(192, 208, 208, 208);
                Text = Color.Black;
                DisabledButtonText = Color.FromArgb(108, 108, 108);
                SynapseBase = Color.LightGray;
                SynapseTip = Color.FromArgb(127, 127, 127);
                EditableTextBackgroundSelected = Color.CornflowerBlue;
                EditableTextBackgroundUnselected = Background;
            }
            else if (mode == Mode.Dark)
            {
                Background = Color.FromArgb(24, 24, 24);
                MenuBackground = Color.FromArgb(192, 48, 48, 48);
                Text = Color.LightGray;
                DisabledButtonText = Color.FromArgb(108, 108, 108);
                SynapseBase = Color.FromArgb(64, 64, 64);
                SynapseTip = Color.FromArgb(127, 127, 127);
                EditableTextBackgroundSelected = Color.FromArgb(64, 64, 128);
                EditableTextBackgroundUnselected = Background;
            }
        }

        public Color GetSynapseBaseTipColor(Color tipColor)
        {
            if (ColorMode == Mode.Light)
            {
                const float lighteningFactor = 0.3f; // lower is lighter
                return Color.FromArgb(
                    255 - (int)((255 - tipColor.R) * lighteningFactor),
                    255 - (int)((255 - tipColor.G) * lighteningFactor),
                    255 - (int)((255 - tipColor.B) * lighteningFactor)
                    );
            }
            else if (ColorMode == Mode.Dark)
            {
                const float darkeningFactor = 0.3f; // lower is darker
                return Color.FromArgb(
                    (int)(tipColor.R * darkeningFactor),
                    (int)(tipColor.G * darkeningFactor),
                    (int)(tipColor.B * darkeningFactor)
                    );
            }
            else
            {
                return tipColor; // robust fallback in the unlikely event a third color mode is added and I forget to update this
            }
        }
    }
}
