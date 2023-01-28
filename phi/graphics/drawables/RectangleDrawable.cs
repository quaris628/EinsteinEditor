using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.other;

namespace phi.graphics.drawables
{
   public class RectangleDrawable : PenDrawable
   {
      public RectangleDrawable(phi.other.Rectangle rect) : base(rect) { }

      public RectangleDrawable(System.Drawing.Rectangle rect) : base(rect) { }

      public RectangleDrawable(int x, int y, int width, int height) : base(x, y, width, height) { }

      protected override void DrawAt(Graphics g, int x, int y)
      {
         g.FillRectangle(GetPen().Brush, (System.Drawing.Rectangle)GetBoundaryRectangle());
      }

      public void SetWidth(int width)
      {
         this.width = width;
         FlagChange();
      }

      public void SetHeight(int height)
      {
         this.height = height;
         FlagChange();
      }
   }
}
