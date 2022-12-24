using System;
using System.Collections.Generic;
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
         public const int WIDTH = 600;
         public const int HEIGHT = 400;
         
      }
      
      public class Render
      {
         public const string DEFAULT_BACKGROUND = RES_DIR + "defaultBackground.png";
         public const int DEFAULT_LAYER = 0;
         public const int FPS = 60;

      }

      public virtual string GetHomeDir() { return HOME_DIR; }
      public virtual string GetResourcesDir() { return RES_DIR; }
      public virtual string GetWindowTitle() { return Window.TITLE; }
      public virtual int GetWindowWidth() { return Window.WIDTH; }
      public virtual int GetWindowHeight() { return Window.HEIGHT; }
      public virtual string GetRenderDefaultBackground() { return Render.DEFAULT_BACKGROUND; }
      public virtual int GetRenderDefaultLayer() { return Render.DEFAULT_LAYER; }
      public virtual int GetRenderFPS() { return Render.FPS; }
   }
}
