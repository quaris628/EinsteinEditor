using phi.graphics;
using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics.Shapes
{
   public class Circle : Shape
   {
      private double radius;
      protected Sprite s;
      public Circle(Sprite s, double originX, double originY, double radius)
      {
         this.updatePosition(originX, originY);
         this.radius = radius;
         this.s = s;
         this.type = ShapeTypes.CIRCLE;
      }
      public Circle(Sprite s, double originX, double originY)
      {
         this.s = s;
         this.updatePosition(originX, originY);
         this.radius = s.GetHeight() / 2;
         this.type = ShapeTypes.CIRCLE;
      }
      public override void updatePosition(double originX, double originY)
      {
         base.updatePosition(originX, originY);
         s.SetXY((int)originX, (int)originY);
      }
      public override Drawable GetDrawable()
      {
         return s;
      }

      public override int GetHeight()
      {
         return (int)(radius * 2);
      }

      public override int GetWidth()
      {
         return (int)(radius * 2); // for circles, width = height
      }

      public override bool isColliding(Shape s)
      {
         if(s.getShapeType() == ShapeTypes.CIRCLE)
         {
            double distance = Math.Sqrt(Math.Pow(this.getX() - s.getX(), 2) + Math.Pow(this.getY() - s.getY(), 2));
            return distance < this.GetHeight() / 2 + s.GetHeight() / 2;
         }
         return false;
      }

      public override bool willCollide(Shape s, double dx, double dy)
      {
         if (s.getShapeType() == ShapeTypes.CIRCLE)
         {
            double distance = Math.Sqrt(Math.Pow(this.getX() + dx - s.getX(), 2) + Math.Pow(this.getY() + dy - s.getY(), 2));
            return distance < this.GetHeight() / 2 + s.GetHeight() / 2;
         }
         return false;
      }

      public override Edge[] GetEdges()
      {
         throw new NotImplementedException();
      }
   }
}
