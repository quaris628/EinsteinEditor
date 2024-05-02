using Einstein.config;
using Einstein.ui.menu;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui
{
    public class KeybindsInfoText : Text
    {
        public const string MESSAGE =
            "------------ Neurons ------------\n" +
            "Add          Menus\n" +
            "Move        Click and drag\n" +
            "Remove    Shift-click\n" +
            "Paint         Ctrl-click\n" +
            "Set color   1-0\n" +
            "Bind color  Ctrl + 1-0\n" +
            "----------- Synapses -----------\n" +
            "Add              Right-click\n" +
            "Remove        Shift-click\n" +
            "Edit strength Click number";

        public KeybindsInfoText(int x, int y) : base(
            new TextBuilder(MESSAGE)
            .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
            .WithFontSize(10f)
            .WithXY(x, y)
            ) { }
    }
}
