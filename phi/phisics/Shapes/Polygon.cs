using phi.graphics;
using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics.Shapes
{
   class Polygon : Shape
   {
      protected Sprite s;
      private Edge[] edges;
      public Polygon(Sprite s, double originX, double originY, Edge[] e)
      {
         this.s = s;
         this.originX = originX;
         this.originY = originY;
         this.type = ShapeTypes.POLYGON;
         edges = e;
         
      }

      public override Drawable GetDrawable()
      {
         return s;
      }

      public override Edge[] GetEdges()
      {
         return edges;
      }

      public override int GetHeight()
      {
         return s.GetHeight();
      }

      public override int GetWidth()
      {
         return s.GetWidth();
      }

      public override bool isColliding(Shape s)
      {
         for (int i = 0; i < edges.Length; i++)
         {
            foreach (Edge e in s.GetEdges())
            {
               if (edges[i].Intersects(e))
                  return true;
            }
         }
         return false;
      }

      public override bool willCollide(Shape s, double dx, double dy)
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
   }
}
