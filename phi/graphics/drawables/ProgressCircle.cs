using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class ProgressCircle : ProgressBar
   {
      private int radius;
      public ProgressCircle(int x, int y, int radius, double max) : base(x, y, radius, radius, max)
      {
         this.radius = radius;
      }
      protected override void DrawAt(Graphics g, int x, int y)
      {
         g.FillPie(new SolidBrush(Color.Black), new Rectangle(x, y, radius, radius), 270, (float)(-360 * (current + min) / max));
      }
   }
}
