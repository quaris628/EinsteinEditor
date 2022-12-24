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
      protected Image background;

      protected Scene(Scene prevScene)
      {
         this.prevScene = prevScene;
         this.background = (Image)IO.RENDERER.GetBackground().Clone();
      }

      protected Scene(Scene prevScene, string imageFile)
      {
         this.prevScene = prevScene;
         this.background = Image.FromFile(imageFile);
      }

      protected Scene(Scene prevScene, ImageWrapper background)
      {
         this.prevScene = prevScene;
         if (background == null) { throw new ArgumentNullException(); }
         this.background = background.GetImage();
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
