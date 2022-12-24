using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.phisics.Shapes
{
   public struct point
   {
      public int x, y;
      public point(int x, int y)
      {
         this.x = x;
         this.y = y;
         
      }
      public point add(int dx, int dy)
      {

         return new point(x + dx, y + dy);
      }
   }
   public class Edge
   {
      private point p1, p2;
      public Edge(point p1, point p2)
      {
         this.p1 = p1;
         this.p2 = p2;
      }
      public Edge(int x1, int y1, int x2, int y2)
      {
         this.p1 = new point(x1, y1);
         this.p2 = new point(x2, y2);
      }
      public point GetPoint1()
      {
         return p1;
      }
      public point GetPoint2()
      {
         return p2;
      }
      public bool Intersects(Edge e)
      {
         return ccw(this.p1, e.p1, e.p2) != ccw(this.p2, e.p1, e.p2) && ccw(this.p1, this.p2, e.p1) != ccw(this.p1, this.p2, e.p2);
      }

      public bool Intersects(Edge e, int dx, int dy)
      {
         return ccw(this.p1.add(dx, dy), e.p1, e.p2) != ccw(this.p2.add(dx, dy), e.p1, e.p2) && ccw(this.p1.add(dx, dy), this.p2.add(dx, dy), e.p1) != ccw(this.p1.add(dx, dy), this.p2.add(dx, dy), e.p2);
      }
      private bool ccw(point p1, point p2, point p3)
      {
         return (p3.y - p1.y) * (p2.x - p1.x) > (p2.y - p1.y) * (p3.x - p1.x);
      }

      public double calculateLength()
      {
         double d = (p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y);
         return Math.Sqrt(d);
      }

      public void shiftEdge(point p)
      {
         p1.x += p.x;
         p1.y += p.y;
         p2.x += p.x;
         p2.y += p.y;
      }
      public Angle getAngle()
      {
         return Angle.CreateSlope(p2.x - p1.x, p2.y - p1.y);
      }
      public Angle getNormal()
      {
         return getAngle().getNormal();
      }
         
      public void shiftEdge(int x1, int y1)
      {
         shiftEdge(new point(x1, y1));
      }
   }
}
