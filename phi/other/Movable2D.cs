using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.other
{
   public class Movable2D : DynamicHoldable
   {
      private int x;
      private int y;

      public Movable2D(int x, int y) { this.x = x; this.y = y; }

      public int GetX() { return x; }
      public int GetY() { return y; }
      public void SetX(int x) { this.x = x; FlagChange(); }
      public void SetY(int y) { this.y = y; FlagChange(); }
      public void SetXY(int x, int y) { this.x = x; this.y = y; FlagChange(); }

      public override int GetHashCode()
      {
         unchecked // allow arithmetic overflow
         {
            int result = 1046527;
            result *= 106033 ^ base.GetHashCode();
            result *= 106033 ^ x;
            result *= 106033 ^ y;
            return result;
         }
      }

      public override bool Equals(object obj)
      {
         if (!base.Equals(obj)) { return false; }
         if (this.GetType() != obj.GetType()) { return false; }
         return (this.x == ((Movable2D)obj).x
              && this.y == ((Movable2D)obj).y);
      }

      public override string ToString()
      {
         return "{" + x.ToString() + "," + y.ToString() + "} : " + GetHashCode().ToString();
      }

   }
}
