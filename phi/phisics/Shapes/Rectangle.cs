using phi.graphics;
using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics.Shapes
{
   public class Rectangle : Shape
   {
      private int height;
      private int width;
      protected Sprite s;
      private Edge[] edges;
      public Rectangle(Sprite s, double originX, double originY, int width, int height)
      {
         this.s = s;
         this.originX = originX;
         this.originY = originY;
         this.width = width;
         this.height = height;
         this.type = ShapeTypes.RECTANGLE;
         createEdges();
      }
      public Rectangle(Sprite s, double originX, double originY)
      {
         this.s = s;
         this.originX = originX;
         this.originY = originY;
         this.width = s.GetWidth();
         this.height = s.GetHeight();
         this.type = ShapeTypes.RECTANGLE;
      }

      public override int GetHeight()
      {
         return height;
      }

      public override int GetWidth()
      {
         return width;
      }
      public override void updatePosition(double originX, double originY)
      {
         base.updatePosition(originX, originY);
         s.SetXY((int)originX, (int)originY);
         createEdges();
      }

      public override bool willCollide(Shape s, double dx, double dy)
      {
         if(s.getShapeType() == ShapeTypes.RECTANGLE)
         {
            
            return ((this.originX + dx < s.getX() + s.GetWidth() && this.originX + dx + this.width > s.getX())
               && (this.originY + dy < s.getY() + s.GetHeight() && this.originY + dy + this.height > s.getY()));
         }
         else
         {
            for (int i = 0; i < edges.Length; i++)
            {
               foreach (Edge e in s.GetEdges())
               {
                  if (edges[i].Intersects(e, (int)dx, (int)dy))
                     return true;
               }
            }
            return false;
         }
         return false;
      }

      public override bool isColliding(Shape s)
      {
         if (s.getShapeType() == ShapeTypes.RECTANGLE) //Less complicated AABB algorithm
         {
            return ((this.originX < s.getX() + s.GetWidth() && this.originX + this.width > s.getX())
                  && (this.originY < s.getY() + s.GetHeight() && this.originY + this.height > s.getY()));
         }
         else
         {
            for (int i = 0; i < edges.Length; i++)
            {
               foreach (Edge e in s.GetEdges())
               {
                  if (edges[i].Intersects(e))
                     return true;
               }
            }
         }
         return false;

      }

      private void createEdges()
      {
         point p1, p2, p3, p4;
         p1 = new point((int)originX, (int)originY);
         p2 = new point((int)originX + width, (int)originY);
         p3 = new point((int)originX, (int)originY + height);
         p4 = new point((int)originX + width, (int)originY+height);
         edges = new Edge[4];
         edges[0] = new Edge(p1, p2);
         edges[1] = new Edge(p1, p3);
         edges[2] = new Edge(p2, p4);
         edges[3] = new Edge(p3, p4);
      }

      public override Edge[] GetEdges()
      {
         return edges;
      }

      public override Drawable GetDrawable()
      {
         return s;
      }
   }
}
