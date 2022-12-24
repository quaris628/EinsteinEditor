using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.other
{
   public struct Rectangle
   {
      public int X;
      public int Y;
      public int Width;
      public int Height;

      public Rectangle(int x, int y, int width, int height)
      {
         X = x;
         Y = y;
         Width = width;
         Height = height;
      }

      public bool Contains(int x, int y)
      {
         return 0 <= x - X && x - X < Width && 0 <= y - Y && y - Y < Height;
      }

      public static explicit operator System.Drawing.Rectangle(Rectangle r)
      {
         return new System.Drawing.Rectangle(r.X, r.Y, r.Width, r.Height);
      }

   }
}
