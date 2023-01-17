﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using phi.control;

namespace phi
{
   public static class PhiMain
   {
       
      [STAThread]
      public static void Main(Scene entryScene, DefaultConfig config)
      {
         Application.ThreadException += new ThreadExceptionEventHandler(HandleThreadException);
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new WindowsForm(entryScene, config));
      }

      [STAThread]
      public static void Main(Scene entryScene)
      {
         Application.ThreadException += new ThreadExceptionEventHandler(HandleThreadException);
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new WindowsForm(entryScene, new DefaultConfig()));
      }

      public static void Main() { }

      // So APPARENTLY Application.Run will catch any unhandled exceptions itself
      // BUT ONLY IN RELEASE BUILDS...
      // Because WHY THE FUCK NOT, apparently
      // So this method is a workaround to that
      // See https://stackoverflow.com/questions/6734287/why-is-my-catch-block-only-running-while-debugging-in-visual-studio/6734767#6734767
      // Fuck whoever made this disabled in debug mode
      // I spent 3 damn hours pounding this out
      private static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
      {
         throw e.Exception;
      }
   }
}
