using System;
using System.Collections.Generic;
using System.Drawing;
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
      int GetMaxWindowWidth();
      int GetMaxWindowHeight();

      Color GetRenderDefaultBackground();
      int GetRenderDefaultLayer();
      int GetRenderFPS();

   }
}
