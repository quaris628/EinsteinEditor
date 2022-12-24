using phi.graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics.Shapes
{
   public enum ShapeTypes
   {
      RECTANGLE,
      CIRCLE,
      POLYGON
   }
   public abstract class Shape : Renderable
   {
      
      protected double originX;
      protected double originY;
      protected ShapeTypes type;
      protected bool movedFlag;
      public abstract int GetHeight();
      public abstract int GetWidth();

      /**
       * Returns whether the shape has moved since the last HasMoved call
       */
      public bool HasMoved()
      {
         if(movedFlag)
         {
            movedFlag = false;
            return true;
         }
         else
         {
            return false;
         }
      }
      public virtual void updatePosition(double originX, double originY)
      {
         this.originX = originX;
         this.originY = originY;
         movedFlag = true;
      }
      public ShapeTypes getShapeType()
      {
         return type;
      }
      public double getX()
      {
         return originX;
      }
      public double getY()
      {
         return originY;
      }
      public abstract Edge[] GetEdges();
      public abstract bool willCollide(Shape s, double dx, double dy);
      public abstract bool isColliding(Shape s);
      public abstract Drawable GetDrawable();
   }
}
