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
         return isBetween(X, x, X + Width) && isBetween(Y, y, Y + Height);
      }

      private bool isBetween(int bound1, int value, int bound2)
      {
         return (bound1 <= value && value < bound2)
            || (bound2 <= value && value < bound1);
      }

      public static explicit operator System.Drawing.Rectangle(Rectangle r)
      {
         return new System.Drawing.Rectangle(r.X, r.Y, r.Width, r.Height);
      }

   }
}
