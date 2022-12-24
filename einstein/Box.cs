using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.phisics.Shapes;
using phi.graphics;
using phi.graphics.drawables;
using phi.io;

namespace Einstein
{
   class Box : Rectangle
   {
      private const string IMAGE = EinsteinPhiConfig.RES_DIR + "Box.png";
      public Box(double originX, double originY, int width, int height) : base(new Sprite(new ImageWrapper(IMAGE), (int)originX, (int)originY), originX, originY, width, height)
      {

      }
      public Box(double originX, double originY) : base(new Sprite(new ImageWrapper(IMAGE), (int) originX, (int)originY), originX, originY)
      {

      }
   }
}
