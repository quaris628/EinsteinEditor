using Einstein.ui.menu;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.visibleElements
{
    public class KeybindsInfoText : Text
    {
        public const string MESSAGE =
            "------------ Neurons ------------\n" +
            "Add          Menus\n" +
            "Remove    Shift-click\n" +
            "Move        Click and drag\n" +
            "----------- Synapses -----------\n" +
            "Add              Right-click\n" +
            "Remove        Shift-click\n" +
            "Edit strength Click number";

        public KeybindsInfoText(int x, int y) : base(
            new TextBuilder(MESSAGE)
            .WithFontSize(10f)
            .WithXY(x, y)
            ) { }
    }
}
