using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi
{
   // untested
   public class SaveableConfig : Config
   {
      private const string DEFAULT_SAVE_FILE = HOME_DIR + "phi-config.txt";

      // make only immutable fields const, otherwise static
      
      // I'm not positive which fields should be mutable during runtime and from file load.
      // The current state is my best guess. -Nathan

      public const string HOME_DIR = DefaultConfig.HOME_DIR;
      public const string RES_DIR = DefaultConfig.RES_DIR;
      
      public class Window
      {
         public static string TITLE = "Phi Engine Application";
         private static int _width = 600;
         public static int WIDTH
         {
            get => _width;
            set
            {
               if (value <= 0) { throw new ArgumentException("Negative window width"); }
               _width = value;
            }
         }
         private static int _height = 600;
         public static int HEIGHT
         {
            get => _height;
            set
            {
               if (value <= 0) { throw new ArgumentException("Negative window height"); }
               _height = value;
            }
         }
      }

      public class Render
      {
         public const string DEFAULT_BACKGROUND = RES_DIR + "defaultBackground.png";
         private static int _default_layer = 0;
         public static int DEFAULT_LAYER
         {
            get => _default_layer;
            set
            {
               if (value <= 0) { throw new ArgumentException("Negative layer"); }
               _default_layer = value;
            }
         }
         private static int _fps = 60;
         public static int FPS
         {
            get => _fps;
            set
            {
               if (value <= 0) { throw new ArgumentException("Negative fps"); }
               if (value > 1000) { throw new ArgumentException("fps too large"); }
               _fps = value;
            }
         }
      }

      // Save
      public void Save() { Save(DEFAULT_SAVE_FILE); }
      public void Save(string path)
      {
         // just throw any exceptions from this
         StreamWriter f = new StreamWriter(path, false, Encoding.UTF8);
         try
         {
            // write values to file
            f.WriteLine(" ----- Phi Config Save File ----- ");
            f.WriteLine("\nWindow");
            f.WriteLine("title:" + Window.TITLE);
            f.WriteLine("width:" + Window.WIDTH);
            f.WriteLine("height:" + Window.HEIGHT);
            f.WriteLine("\nRenderer");
            f.WriteLine("default-layer:" + Render.DEFAULT_LAYER);
            f.WriteLine("fps:" + Render.FPS);
         }
         catch (Exception e)
         {
            Console.WriteLine("Exception: " + e.Message);
         }
         finally
         {
            f.Close();
         }
      }

      // Load
      public void Load() { Load(DEFAULT_SAVE_FILE); }
      public void Load(string path)
      {
         // just throw any exceptions from this
         StreamReader f = new StreamReader(path, Encoding.UTF8, true);
         try
         {
            // read strings from file
            f.ReadLine(); //  ----- Phi Config File ----- 
            f.ReadLine(); // 
            f.ReadLine(); // Window
            string titleLine = f.ReadLine(); // title:TITLE
            string widthLine = f.ReadLine(); // width:WIDTH
            string heightLine = f.ReadLine(); // height:HEIGHT
            f.ReadLine(); // Renderer
            string layerLine = f.ReadLine(); // default-layer:DEFAULT_LAYER
            string fpsLine = f.ReadLine(); // fps:FPS

            // parse strings to values
            Window.TITLE = SubstringAfter(titleLine, ':');
            Window.WIDTH = int.Parse(SubstringAfter(widthLine, ':'));
            Window.HEIGHT = int.Parse(SubstringAfter(heightLine, ':'));
            Render.DEFAULT_LAYER = int.Parse(SubstringAfter(layerLine, ':'));
            Render.FPS = int.Parse(SubstringAfter(fpsLine, ':'));
         }
         catch (Exception e)
         {
            Console.WriteLine("Exception reading file: " + e.Message);
         }
         finally
         {
            f.Close();
         }
      }

      private string SubstringAfter(string str, char delimiter)
      {
         return str.Substring(str.IndexOf(delimiter) + 1);
      }


      public virtual void RestoreDefaults()
      {
         //Restores all Default values from the DefaultConfig
         Window.TITLE = DefaultConfig.Window.TITLE;
         Window.WIDTH = DefaultConfig.Window.WIDTH;
         Window.HEIGHT = DefaultConfig.Window.HEIGHT;
         Render.DEFAULT_LAYER = DefaultConfig.Render.DEFAULT_LAYER;
         Render.FPS = DefaultConfig.Render.FPS;
      }

      public virtual void RestoreDefaultResolution()
      {
         //Restores just the resolution to default
         Window.WIDTH = DefaultConfig.Window.WIDTH;
         Window.HEIGHT = DefaultConfig.Window.HEIGHT;
      }

      // Implement Config interface
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
