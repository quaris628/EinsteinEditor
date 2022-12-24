using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi;

namespace Einstein
{
   static class EinsteinMain
   {
      // to enable reference to the phi namescape:
      // in VS solution explorer, project -> references -> Add reference

      // to hide console for a project (this has already been done for this project):
      // in VS, project -> properties -> Output type, set to Windows Application instead of Console Application
	   
      // Entry point
      public static void Main()
      {
         PhiMain.Main(new EditorScene(null), new EinsteinPhiConfig());
      }
   }
}
