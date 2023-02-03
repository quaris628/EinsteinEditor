using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.other;

namespace phi.graphics
{
   public abstract class Drawable : Movable2D
   {
      private static int nextId = 0;
      private int id;
      private bool displaying;
      protected int height;
      protected int width;

      public Drawable(int x, int y, int width, int height) : base(x, y)
      {
         id = nextId++;
         displaying = true;
         this.height = height;
         this.width = width;
      }

      public Drawable(other.Rectangle rect) : base(rect.X, rect.Y)
      {
         id = nextId++;
         displaying = true;
         this.height = rect.Height;
         this.width = rect.Width;
      }

      public Drawable(System.Drawing.Rectangle rect) : base(rect.X, rect.Y)
      {
         id = nextId++;
         displaying = true;
         this.height = rect.Height;
         this.width = rect.Width;
      }

      public void Draw(Graphics g) { DrawAt(g, this.GetX(), this.GetY()); }
      public void DrawOffset(Graphics g, int xOffset, int yOffset) { DrawAt(g, this.GetX() + xOffset, this.GetY() + yOffset); }
      protected abstract void DrawAt(Graphics g, int x, int y);

      public virtual int GetHeight() { return height; }
      public virtual int GetWidth() { return width; }
      public virtual void SetCenterX(int x) { SetX(x - GetWidth() / 2); }
      public virtual void SetCenterY(int y) { SetY(y - GetHeight() / 2); }
      public virtual void SetCenterXY(int x, int y) { SetCenterX(x); SetCenterY(y); }
      public virtual int GetCenterX() { return GetX() + GetWidth() / 2; }
      public virtual int GetCenterY() { return GetY() + GetHeight() / 2; }
      public virtual int[] GetCenter() { return new int[] { GetCenterX(), GetCenterY() }; }
      public virtual phi.other.Rectangle GetBoundaryRectangle()
      {
         return new phi.other.Rectangle(GetX(), GetY(), GetWidth(), GetHeight());
      }

      public virtual void SetDisplaying(bool displaying) { this.displaying = displaying; FlagChange(); }
      public virtual bool IsDisplaying() { return displaying; }

      public override int GetHashCode()
      {
         return id;
      }

      public override bool Equals(object obj)
      {
         if (!base.Equals(obj)) { return false; }
         if (this.GetType() != obj.GetType()) { return false; }
         return this.GetHashCode() == obj.GetHashCode();
      }

      public override string ToString()
      {
         return (displaying ? "Showing" : "Hiding") + " at " + base.ToString();
      }
   }
}
