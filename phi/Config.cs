using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi
{
   public interface Config
   {
      string GetHomeDir();
      string GetResourcesDir();

      string GetWindowTitle();
      int GetWindowWidth();
      int GetWindowHeight();

      string GetRenderDefaultBackground();
      int GetRenderDefaultLayer();
      int GetRenderFPS();

   }
}
