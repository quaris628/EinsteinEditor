using phi.phisics.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class Line : PenDrawable
   {
      public Line(int startX, int startY, int endX, int endY)
         : base(startX, startY, endX - startX, endY - startY) { }

      public Line(Edge e)
      : base(e.GetPoint1().x, e.GetPoint1().y, e.GetPoint2().x - e.GetPoint1().x, e.GetPoint2().y - e.GetPoint1().y) { }

      protected override void DrawAt(Graphics g, int x, int y)
      {
         int x2 = x + GetWidth();
         int y2 = y + GetHeight();
         g.DrawLine(GetPen(), x, y, x2, y2);
      }

      public void SetXY1(int x, int y) { SetX1(x); SetY1(y); }
      public void SetX1(int x) { this.width -= x - GetX(); SetX(x); }
      public void SetY1(int y) { this.height -= y - GetY(); SetY(y); }

      public void SetXY2(int x, int y) { SetX2(x); SetY2(y); }
      public void SetX2(int x) { this.width = x - GetX(); FlagChange(); }
      public void SetY2(int y) { this.height = y - GetY(); FlagChange(); }

      public override string ToString()
      {
         return "Line " + base.ToString();
      }
   }
}
