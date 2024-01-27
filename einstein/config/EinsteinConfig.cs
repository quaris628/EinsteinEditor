using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Einstein.ui;
using phi;

namespace Einstein.config
{
    // pass this object to the PhiMain method to set it to be used as the phi config settings
    public class EinsteinConfig : DefaultConfig
    {
        public new const string HOME_DIR = "../";
        public new const string RES_DIR = HOME_DIR + "res/";
        public const int PAD = 5;
        public const string VERSION = "v1.6.1 - dev";

        public static ColorScheme COLOR_MODE = new ColorScheme(ColorScheme.Mode.Dark);

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
