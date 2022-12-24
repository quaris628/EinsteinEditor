using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.io;
using phi.graphics;
using phi.control;

namespace phi.io
{
   public class IO
   {
      public static readonly FrameTimerInputHandler FRAME_TIMER = new FrameTimerInputHandler();
      public static readonly KeyInputHandler KEYS = new KeyInputHandler();
      public struct MOUSE
      {
         public static readonly MouseInputHandler CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler UP = new MouseInputHandler();
         public static readonly MouseInputHandler MOVE = new MouseInputHandler();
         public static void Clear() { CLICK.Clear(); DOWN.Clear(); UP.Clear(); MOVE.Clear(); }
      }
      public static readonly Renderer RENDERER = new Renderer();

      private IO() { }

      public static void Clear()
      {
         FRAME_TIMER.Clear();
         KEYS.Clear();
         MOUSE.Clear();
         RENDERER.Clear();
      }

      public static void Exit() { WindowsForm.Exit(); }
   }
}
