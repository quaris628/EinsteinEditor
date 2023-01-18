using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.control;

namespace phi
{
   public class DefaultConfig : Config
   {
      public const string HOME_DIR = "../../";
      public const string RES_DIR = HOME_DIR + "res/";

      public class Window
      {
         public const string TITLE = "Phi Engine";
         public const int WIDTH = 2048;
         public const int HEIGHT = 1080;

      }
      
      public class Render
      {
         public static readonly Color DEFAULT_BACKGROUND = Color.White;
         public const int DEFAULT_LAYER = 10;
         public const int FPS = 60;

      }

      public virtual string GetHomeDir() { return HOME_DIR; }
      public virtual string GetResourcesDir() { return RES_DIR; }
      public virtual string GetWindowTitle() { return Window.TITLE; }
      public virtual int GetMaxWindowWidth() { return Window.WIDTH; }
      public virtual int GetMaxWindowHeight() { return Window.HEIGHT; }
      public virtual Color GetRenderDefaultBackground() { return Render.DEFAULT_BACKGROUND; }
      public virtual int GetRenderDefaultLayer() { return Render.DEFAULT_LAYER; }
      public virtual int GetRenderFPS() { return Render.FPS; }
   }
}
