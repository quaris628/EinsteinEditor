using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.io;
using phi.graphics;
using phi.control;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace phi.io
{
   public partial class IO
   {
      public static readonly FrameTimerInputHandler FRAME_TIMER = new FrameTimerInputHandler();
      public static readonly KeyInputHandler KEYS = new KeyInputHandler();
      public struct MOUSE
      {
         public static readonly MouseInputHandler CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK = new MouseInputHandler();
         public static readonly MouseInputHandler DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler UP = new MouseInputHandler();
         public static readonly MouseInputHandler LEFT_UP = new MouseInputHandler();
         public static readonly MouseInputHandler RIGHT_UP = new MouseInputHandler();
         public static readonly MouseInputHandler MID_CLICK_UP = new MouseInputHandler();
         public static readonly MouseInputHandler MID_SCROLL = new MouseInputHandler();
         public static readonly MouseInputHandler MID_SCROLL_DOWN = new MouseInputHandler();
         public static readonly MouseInputHandler MID_SCROLL_UP = new MouseInputHandler();
         public static readonly MouseInputHandler MOVE = new MouseInputHandler();
         public static void Clear()
         {
            CLICK.Clear();
            LEFT_CLICK.Clear();
            RIGHT_CLICK.Clear();
            MID_CLICK.Clear();
            DOWN.Clear();
            LEFT_DOWN.Clear();
            RIGHT_DOWN.Clear();
            MID_CLICK_DOWN.Clear();
            UP.Clear();
            LEFT_UP.Clear();
            RIGHT_UP.Clear();
            MID_CLICK_UP.Clear();
            //MID_SCROLL_DOWN.Clear();
            //MID_SCROLL_UP.Clear();
            MOVE.Clear();
         }
      }
      public static readonly Renderer RENDERER = new Renderer();
      
      public struct WINDOW
      {
         private static WindowsForm form;
         internal static void SetWindowsForm(WindowsForm form) { WINDOW.form = form; }
         public static int GetWidth() { return form.Width - WindowsForm.WIDTH_FUDGE; }
         public static int GetHeight() { return form.Height - WindowsForm.HEIGHT_FUDGE; }
      }

      private IO() { }

      public static void Clear()
      {
         FRAME_TIMER.Clear();
         KEYS.Clear();
         MOUSE.Clear();
         RENDERER.Clear();
      }

      public static void Exit() { WindowsForm.Exit(); }

      public static string LogDetailsForCrash()
      {
         string log = "\n\n ---------------- phi.IO ----------------\n";
         try { log += FRAME_TIMER.LogDetailsForCrash(); } catch (Exception) { log += "\nFailed to log details for FRAME_TIMER."; }
         try { log += KEYS.LogDetailsForCrash(); } catch (Exception) { log += "\nFailed to log details for KEYS."; }
         log += "\nMOUSE.CLICK:";
         try { log += MOUSE.CLICK.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.CLICK:";
         try { log += MOUSE.LEFT_CLICK.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.RIGHT_CLICK:";
         try { log += MOUSE.RIGHT_CLICK.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.MID_CLICK:";
         try { log += MOUSE.MID_CLICK.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.DOWN:";
         try { log += MOUSE.DOWN.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.LEFT_DOWN:";
         try { log += MOUSE.LEFT_DOWN.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.RIGHT_DOWN:";
         try { log += MOUSE.RIGHT_DOWN.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.MID_CLICK_DOWN:";
         try { log += MOUSE.MID_CLICK_DOWN.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.UP:";
         try { log += MOUSE.UP.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.LEFT_UP:";
         try { log += MOUSE.LEFT_UP.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.RIGHT_UP:";
         try { log += MOUSE.RIGHT_UP.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.MID_CLICK_UP:";
         try { log += MOUSE.MID_CLICK_UP.LogDetailsForCrash(); } catch (Exception) { }
         log += "\nMOUSE.MOVE:";
         try { log += MOUSE.MOVE.LogDetailsForCrash(); } catch (Exception) { }
         
         return log;
      }
   }
}
