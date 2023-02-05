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
      string GetWindowIcon();
      int GetMaxWindowWidth();
      int GetMaxWindowHeight();
      int GetInitialWindowWidth();
      int GetInitialWindowHeight();

      Color GetRenderDefaultBackground();
      int GetRenderDefaultLayer();
      int GetRenderFPS();

   }
}
