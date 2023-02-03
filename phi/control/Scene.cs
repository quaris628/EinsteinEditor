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
      protected bool isInit;

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
         isInit = true;
      }
      protected virtual void InitializeMe() { }

      public void Uninitialize()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         isInit = false;
         IO.Clear();
         UninitializeMe();
      }
      protected virtual void UninitializeMe() { }

      protected void SwitchTo(Scene scene)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (scene == null)
         {
            IO.Exit();
         }
         else if (!this.Equals(scene))
         {
            IO.FRAME_TIMER.QueueUninit(this.Uninitialize);
            scene.Initialize();
         }
      }
      protected void Back()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         SwitchTo(prevScene);
      }
   }
}
