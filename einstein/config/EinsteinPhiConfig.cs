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
        public new const string HOME_DIR = "../";
        public new const string RES_DIR = HOME_DIR + "res/";
        public const int PAD = 5;
        public const string VERSION = "v0.0.0";

        public new class Window : DefaultConfig.Window
        {
            public new const string TITLE = "Einstien Bibite Editor";
            public new const int WIDTH = 1280;
            public new const int HEIGHT = 720;
        }
        public new class Render : DefaultConfig.Render
        {
            public new const string DEFAULT_BACKGROUND = RES_DIR + "defaultBackground.png";
            public new const int FPS = 30;
        }

        public override string GetHomeDir() { return HOME_DIR; }
        public override string GetResourcesDir() { return RES_DIR; }
        public override int GetWindowWidth() { return Window.WIDTH; }
        public override int GetWindowHeight() { return Window.HEIGHT; }
        public override string GetWindowTitle() { return Window.TITLE; }
        public override string GetRenderDefaultBackground() { return Render.DEFAULT_BACKGROUND; }
        public override int GetRenderFPS() { return Render.FPS; }
    }
}
