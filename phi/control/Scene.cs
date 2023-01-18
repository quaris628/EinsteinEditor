using System;
using System.Drawing;
using phi.io;


namespace phi.control
{
   /**
    * Scene, to be passed control events to from the Windows Form
    *    and to set objects being displayed in the renderer
    * Input events that get passed are KeyDown and FrameTick
    * Output Drawables must be .add-ed to the Renderer
    * @author Nathan Swartz
    */
   public abstract class Scene
   {
      protected Scene prevScene;
      protected Color background;

      protected Scene(Scene prevScene)
      {
         this.prevScene = prevScene;
         this.background = IO.RENDERER.GetBackground();
      }

      protected Scene(Scene prevScene, Color background)
      {
         this.prevScene = prevScene;
         this.background = background;
      }

      public void Initialize()
      {
         IO.RENDERER.SetBackground(background);
         IO.FRAME_TIMER.Subscribe(IO.RENDERER.Render);
         InitializeMe();
      }
      protected virtual void InitializeMe() { }

      public void Close()
      {
         IO.Clear();
         CloseMe();
      }
      protected virtual void CloseMe() { }

      protected void SwitchTo(Scene scene)
      {
         if (scene == null)
         {
            IO.Exit();
         }
         else if (!this.Equals(scene))
         {
            this.Close();
            scene.Initialize();
         }
      }
      protected void Back()
      {
         SwitchTo(prevScene);
      }
   }
}
