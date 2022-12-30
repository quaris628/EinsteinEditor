﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Einstein.ui;
using phi;

namespace Einstein
{
    // pass this object to the PhiMain method to set it to be used as the phi config settings
    public class EinsteinPhiConfig : DefaultConfig
    {
        public const int PAD = 5;

        public new class Window : DefaultConfig.Window
        {
            public new const string TITLE = "Einstien Bibite Editor";
            public new const int WIDTH = 192 + (NeuronDrawable.CIRCLE_DIAMETER + PAD) * 33 / 2 + 2 * PAD;
            public new const int HEIGHT = 600;
        }
        public new class Render : DefaultConfig.Render
        {
            public new const int FPS = 30;
        }

        public override int GetWindowWidth() { return Window.WIDTH; }
        public override int GetWindowHeight() { return Window.HEIGHT; }
        public override string GetWindowTitle() { return Window.TITLE; }
        public override int GetRenderFPS() { return Render.FPS; }
    }
}
