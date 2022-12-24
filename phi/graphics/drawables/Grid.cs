using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class Grid : PenDrawable
   {
      private LinkedList<Line> vertLines;
      private LinkedList<Line> horzLines;

      public Grid(int startX, int startY, int colWidth, int rowHeight, int numCols, int numRows)
         : base(startX, startY, numRows * rowHeight, numCols * colWidth)
      {
         vertLines = new LinkedList<Line>();
         horzLines = new LinkedList<Line>();
         int endX = startX + numCols * colWidth;
         int endY = startY + numRows * rowHeight;
         // vertical lines
         for (int i = startX; i <= endX; i += colWidth)
         {
            vertLines.AddFirst(new Line(i, startY, i, endY));
            vertLines.First.Value.SetPen(GetPen());
         }
         // horizontal lines
         for (int j = startY; j <= endY; j += rowHeight)
         {
            horzLines.AddFirst(new Line(startX, j, endX, j));
            vertLines.First.Value.SetPen(GetPen());
         }
      }

      protected override void DrawAt(Graphics g, int x, int y)
      {
         // draw each line offset by the offset that x,y is from this.x, this.y
         // that is, offset by x - this.x, y - this.y

         // vertical lines
         foreach (Line l in vertLines)
         {
            l.DrawOffset(g, x - GetX(), y - GetY());
         }
         // horizontal lines
         foreach (Line l in horzLines)
         {
            l.DrawOffset(g, x - GetX(), y - GetY());
         }
      }

      public override PenDrawable SetPen(Pen pen)
      {
         base.SetPen(pen); // does this really call PenDrawable.SetPen() ? Or does this call Grid.SetPen()?
         foreach (Line l in vertLines) { l.SetPen(pen); }
         foreach (Line l in horzLines) { l.SetPen(pen); }
         FlagChange();
         return this;
      }

      public override string ToString()
      {
         return "Grid " + base.ToString();
      }
   }
}
