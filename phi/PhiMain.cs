using System;
using System.Collections.Generic;
using System.Linq;
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
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new WindowsForm(entryScene, config));
      }

      [STAThread]
      public static void Main(Scene entryScene)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new WindowsForm(entryScene, new DefaultConfig()));
      }

      public static void Main() { }
   }
}
