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

        public KeybindsInfoText() : base(
            new TextBuilder(MESSAGE)
            .WithFontSize(10f)
            .WithX(EinsteinPhiConfig.PAD)
            .WithY(EinsteinPhiConfig.PAD * 8 + NeuronMenuButton.HEIGHT * 5)
            ) { }

        public void Initialize()
        {
            IO.RENDERER.Add(this);
        }
    }
}
