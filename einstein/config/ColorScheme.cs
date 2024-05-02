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

        public readonly Color Background;
        public readonly Color MenuBackground;
        public readonly Color Text;
        public readonly Color DisabledButtonText;
        public readonly Color SynapseBase;
        public readonly Color SynapseTip;
        public readonly Color SynapseTextBackgroundSelected;
        public readonly Color SynapseTextBackgroundUnselected;

        public ColorScheme(Mode mode)
        {
            if (mode == Mode.Light)
            {
                Background = Color.White;
                MenuBackground = Color.FromArgb(192, 208, 208, 208);
                Text = Color.Black;
                DisabledButtonText = Color.FromArgb(108, 108, 108);
                SynapseBase = Color.LightGray;
                SynapseTip = Color.FromArgb(127, 127, 127);
                SynapseTextBackgroundSelected = Color.CornflowerBlue;
                SynapseTextBackgroundUnselected = Background;
            }
            else if (mode == Mode.Dark)
            {
                Background = Color.FromArgb(24, 24, 24);
                MenuBackground = Color.FromArgb(192, 48, 48, 48);
                Text = Color.LightGray;
                DisabledButtonText = Color.FromArgb(108, 108, 108);
                SynapseBase = Color.FromArgb(64, 64, 64);
                SynapseTip = Color.FromArgb(127, 127, 127);
                SynapseTextBackgroundSelected = Color.FromArgb(64, 64, 128);
                SynapseTextBackgroundUnselected = Background;
            }
        }
    }
}
