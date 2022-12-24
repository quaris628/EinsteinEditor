using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.io
{
   public class ImageWrapper
   {
      private Image image;
      public ImageWrapper(string filename)
      {
         image = Image.FromFile(filename);
      }
      public Image GetImage() { return image; }
   }
}
