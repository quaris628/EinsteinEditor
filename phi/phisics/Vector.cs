using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics
{
   public class Vector
   {
      // the Zero Vector
      public static readonly Vector ZERO = new Vector(0, 0);

      private double magnitude;
      private Angle direction;

      private double xComp;
      private double yComp;

      public Vector(double magnitude, Angle direction)
      {
         this.magnitude = Math.Abs(magnitude);
         if (magnitude > 0)
         {
            this.direction = direction;
         }
         else if (magnitude == 0)
         {
            this.direction = Angle.UNDEFINED;
         }
         else
         {
            this.direction = direction.getOpposite();
         }
         this.direction = magnitude >= 0 ? direction : Angle.CreateDegrees(direction.GetDegrees() + 180);
         yComp = Math.Sin(direction.GetRadians()) * magnitude;
         xComp = Math.Cos(direction.GetRadians()) * magnitude;
      }

      public Vector(double xComp, double yComp)
      {
         this.xComp = xComp;
         this.yComp = yComp;
         if (this.xComp == 0 && this.yComp == 0)
         {
            magnitude = 0;
            direction = Angle.CreateRadians(0);
         }
         else
         {
            magnitude = Math.Sqrt(xComp * xComp + yComp * yComp);
            direction = Angle.CreateSlope(yComp, xComp);
         }
      }

      public void setMagnitude(double magnitude)
      {
         this.magnitude = magnitude;
         xComp = Math.Cos(direction.GetRadians()) * magnitude;
         yComp = Math.Sin(direction.GetRadians()) * magnitude;
      }

      public void setDirection(Angle direction)
      {
         this.direction = direction;
         xComp = Math.Cos(this.direction.GetRadians()) * magnitude;
         yComp = Math.Sin(this.direction.GetRadians()) * magnitude;
      }

      public void setXComp(double xComp)
      {
         this.xComp = xComp;
         magnitude = Math.Sqrt(xComp * xComp + yComp * yComp);
         direction = Angle.CreateSlope(yComp, xComp);
      }

      public void setYComp(double yComp)
      {
         this.yComp = yComp;
         magnitude = Math.Sqrt(xComp * xComp + yComp * yComp);
         direction = Angle.CreateSlope(yComp, xComp);
      }

      public void setXYComp(double xComp, double yComp)
      {
         this.xComp = xComp;
         this.yComp = yComp;
         magnitude = Math.Sqrt(xComp * xComp + yComp * yComp);
         direction = Angle.CreateSlope(yComp, xComp);
      }

      public Angle getDirection() { return direction; }
      public double getMagnitude() { return magnitude; }
      public double getXComp() { return xComp; }
      public double getYComp() { return yComp; }
      
      
      public static implicit operator bool(Vector a)
      {
         return a != null;
      }

      public static implicit operator double(Vector a)
      {
         return a.magnitude;
      }
      
      public static Vector operator +(Vector a) => a;

      public static Vector operator -(Vector a) => new Vector(-a.magnitude, a.direction);

      public static Vector operator +(Vector a, Vector b)
         => new Vector(a.xComp + b.xComp, a.yComp + b.yComp);

      public static Vector operator -(Vector a, Vector b)
         => a + -b;

      public static bool operator ==(Vector a, Vector b)
         => a.xComp == b.xComp && a.yComp == b.yComp;

      public static bool operator !=(Vector a, Vector b)
         => !(a==b);

      public override bool Equals(object obj)
      {
         return (obj.GetType() == typeof(Vector)) && (this == (Vector)obj);
      }
   }
}
