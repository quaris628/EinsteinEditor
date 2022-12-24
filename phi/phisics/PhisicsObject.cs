using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics;
using phi.phisics.Shapes;
using phi.graphics.drawables;

namespace phi.phisics
{
   public enum CollidableType
   {

   }
   public class PhisicsObject : Renderable
   {
      private int x, y;
      private double mass;
      private Shape shape;
      private Vector velocity;
      private HashSet<Force> forces;
      protected PhisicsObject(int x, int y, double mass, Shape shape, Vector velocity)
      {
         this.x = x;
         this.y = y;
         this.mass = mass;
         this.shape = shape;
         this.velocity = velocity;
      }

      protected PhisicsObject(int x, int y, double mass, Shape shape)
      {
         this.x = x;
         this.y = y;
         this.mass = mass;
         this.shape = shape;
         velocity = Vector.ZERO;
      }

      protected PhisicsObject(int x, int y, double mass, Sprite s)
      {
         this.x = x;
         this.y = y;
         this.mass = mass;
         this.shape = new Rectangle(s, x, y);
         velocity = Vector.ZERO;
      }



      public int getX()
      {
         return x;
      }
      public int getY()
      {
         return y;
      }

      public double getMass()
      {
         return mass;
      }
      public Vector getVelocity()
      {
         return velocity;
      }
      
      public Shape getShape()
      {
         return shape;
      }

      //this was originally used for testing purposes, it adds the velocity to it's position, basically used for the tick event.
      public void update(int tickSpeed)
      {
         x += (int)velocity.getXComp();
         y += (int)velocity.getYComp();
         shape.updatePosition(x, y);
      }

      public Drawable GetDrawable()
      {
         return shape.GetDrawable();
      }
   }
}
