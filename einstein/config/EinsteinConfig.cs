using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Einstein.ui;
using phi;
using phi.io;

namespace Einstein.config
{
    // pass this object to the PhiMain method to set it to be used as the phi config settings
    public class EinsteinConfig : DefaultConfig
    {
        public new const string HOME_DIR = "../";
        public new const string RES_DIR = HOME_DIR + "res/";
        public const int PAD = 5;
        public const string VERSION = "v1.7.2 - dev";

        public static readonly ColorScheme COLOR_MODE = new ColorScheme(ColorScheme.Mode.Dark);

        public class Keybinds
        {
            public static readonly KeyStroke LOAD_FROM_BIBITE =
                new KeyStroke.KeyStrokeBuilder(Keys.O).with(Keys.Control).Build();
            public static readonly KeyStroke SAVE_BIBITE =
                new KeyStroke.KeyStrokeBuilder(Keys.S).with(Keys.Control).Build();
            public static readonly KeyStroke SAVE_TO_BIBITE =
                new KeyStroke.KeyStrokeBuilder(Keys.S).with(Keys.Control).with(Keys.Shift).Build();

            public static readonly Keys[] SELECT_PAINT_COLORS = new Keys[] {
                Keys.D1,
                Keys.D2,
                Keys.D3,
                Keys.D4,
                Keys.D5,
                Keys.D6,
                Keys.D7,
                Keys.D8,
                Keys.D9,
                Keys.D0,
            };
            public static readonly Keys REBIND_PAINT_COLOR_MODIFIER = Keys.Control;
        }

        public new class Window : DefaultConfig.Window
        {
            public new const string TITLE = "Einstein Editor " + VERSION;
            public new const string ICON = RES_DIR + "einstein.ico";
        }
        public new class Render : DefaultConfig.Render
        {
            public new const int FPS = 40;
        }

        public override string GetHomeDir() { return HOME_DIR; }
        public override string GetResourcesDir() { return RES_DIR; }
        public override string GetWindowTitle() { return Window.TITLE; }
        public override string GetWindowIcon() { return Window.ICON; }
        public override int GetRenderFPS() { return Render.FPS; }
    }
}
