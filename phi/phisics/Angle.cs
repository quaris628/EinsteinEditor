using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics
{
   public class Angle
   {
      private const double DEGREES_TO_RADIANS = Math.PI / 180;
      private const double RADIANS_TO_DEGREES = 180 / Math.PI;
      private const double TAU = Math.PI * 2;

      private double radians;

      public static readonly Angle UNDEFINED = new Angle(Double.NaN);
      public static Angle CreateRadians(double radians) { return new Angle(radians); }
      public static Angle CreateDegrees(double degrees) { return new Angle(degrees * DEGREES_TO_RADIANS); }
      public static Angle CreateSlope(double rise, double run)
      {
         double radians = run == 0 ? Double.NaN : Math.Atan2(rise, run);
         return new Angle(radians);
      }

      private Angle(double radians)
      {
         this.radians = radians % TAU;
         // C# modulo is truncated definition; we want the floored definition, so this fixes that
         if (radians < 0) { this.radians += TAU; }
      }

      public bool IsDefined()
      {
         return ! (Double.IsNaN(radians) || Double.IsInfinity(radians));
      }

      /**
       * Returns the right angle to the angle
       */
      public Angle getNormal()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return new Angle(radians + Math.PI / 2);
      }

      /**
       * Returns the angle parallel to this angle that is not this angle
       */
      public Angle getOpposite()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return new Angle(radians + Math.PI);
      }

      /**
       * Returns the right angle to the angle
       */
      public Angle getAntiNormal()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return new Angle(radians - Math.PI / 2);
      }

      /**
       * Should output a value between 0 and tau
       */
      public double GetRadians()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return radians;
      }
      /**
       * Output between 0 and 360
       */
      public double GetDegrees()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return radians * RADIANS_TO_DEGREES;
      }
      /**
       * Output any double value
       */
      public double GetSlope()
      {
         if (!IsDefined()) { throw new AngleNotDefinedException(); }
         return Math.Tan(radians);
      }

      public static implicit operator bool(Angle a)
      {
         return a != null;
      }

      public static Angle operator +(Angle a)
         => a;
      public static Angle operator -(Angle a)
         => new Angle(a.radians * -1);
      public static Angle operator +(Angle a, Angle b)
         => new Angle(a.radians + b.radians);
      public static Angle operator -(Angle a, Angle b)
         => a + -b;
      public static bool operator ==(Angle a, Angle b)
         => a.radians == b.radians;
      public static bool operator !=(Angle a, Angle b)
         => !(a == b);

      public override bool Equals(object obj)
      {
         Angle b = obj as Angle;
         if(!b)
         {
            return false;
         }
         else
         {
            return this == b;
         }
      }

      public override int GetHashCode()
      {
         unchecked // allow arithmetic overflow
         {
            long radiansBits = BitConverter.DoubleToInt64Bits(radians);
            return ((int)radiansBits) ^ ((int)radiansBits >> 32);
         }
      }
   }

   public class AngleNotDefinedException : Exception
   {
      public AngleNotDefinedException() : base() { }
      public AngleNotDefinedException(string message) : base(message) { }
   }
}
