using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.io;
using phi.other;


namespace phi.graphics.renderables
{
   public class Draggable : Renderable
   {
      private const bool DEFAULT_USE_RIGHT_MOUSE_BUTTON = false;

      private Drawable drawable;
      private int dragOffsetX;
      private int dragOffsetY;
      private bool hasBounds;
      private Rectangle dragBounds;
      private bool useRightMouseButton;

      public Draggable(Drawable drawable)
         : this(drawable, new Rectangle(), DEFAULT_USE_RIGHT_MOUSE_BUTTON) { }
      public Draggable(Drawable drawable, bool useRightMouseButton)
      {
         this.drawable = drawable;
         this.hasBounds = true;
         this.dragBounds = new Rectangle(); // null doesn't work
         this.useRightMouseButton = useRightMouseButton;
      }
      public Draggable(Drawable drawable, Rectangle rectangle)
         : this(drawable, rectangle, DEFAULT_USE_RIGHT_MOUSE_BUTTON) { }
      public Draggable(Drawable drawable, int minX, int minY, int maxX, int maxY)
         : this(drawable, minX, minY, maxX, maxY, DEFAULT_USE_RIGHT_MOUSE_BUTTON) { }
      public Draggable(Drawable drawable, int minX, int minY, int maxX, int maxY, bool useRightMouseButton)
         : this(drawable, new Rectangle(minX, minY, maxX - minX, maxY - minY), useRightMouseButton) { }
      public Draggable(Drawable drawable, Rectangle dragBounds, bool useRightMouseButton)
      {
         this.drawable = drawable;
         this.hasBounds = true;
         this.dragBounds = dragBounds;
         this.useRightMouseButton = useRightMouseButton;
      }

      public void Initialize()
      {
         IO.MOUSE.DOWN.SubscribeOnDrawable(MouseDown, drawable);
      }

      private void MouseDown(int x, int y)
      {
         IO.MOUSE.MOVE.Subscribe(MouseMove);
         IO.MOUSE.UP.Subscribe(MouseUp);
         dragOffsetX = x - drawable.GetX();
         dragOffsetY = y - drawable.GetY();
         MyMouseDown(x, y);
      }
      protected virtual void MyMouseDown(int x, int y) { }

      private void MouseMove(int x, int y)
      {
         if (hasBounds)
         {
            drawable.SetX(ConfineToXBound(x - dragOffsetX));
            drawable.SetY(ConfineToYBound(y - dragOffsetY));
         }
         else
         {
            drawable.SetXY(x - dragOffsetX, y - dragOffsetY);
         }
         MyMouseMove(x, y);
      }
      protected virtual void MyMouseMove(int x, int y) { }

      private void MouseUp(int x, int y)
      {
         IO.MOUSE.MOVE.Unsubscribe(MouseMove);
         IO.MOUSE.UP.Unsubscribe(MouseUp);
         MyMouseUp(x, y);
      }
      protected virtual void MyMouseUp(int x, int y) { }

      protected int GetDragOffsetX() { return dragOffsetX; }
      protected int GetDragOffsetY() { return dragOffsetY; }

      private int ConfineToXBound(int x)
      {
         return Math.Max(dragBounds.X, Math.Min(x,
            dragBounds.X + dragBounds.Width - drawable.GetWidth()));
      }

      private int ConfineToYBound(int y)
      {
         return Math.Max(dragBounds.Y, Math.Min(y,
            dragBounds.Y + dragBounds.Height - drawable.GetHeight()));
      }

      public bool HasDragBounds() { return this.hasBounds; }
      public void SetDragUnbounded() { this.hasBounds = false; }
      
      public void SetDragBounds(Rectangle bounds)
      {
         this.hasBounds = true;
         this.dragBounds = bounds;
      }

      public void SetDragBounds(int minX, int minY, int maxX, int maxY)
      {
         this.hasBounds = true;
         this.dragBounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
      }

      public Drawable GetDrawable() { return drawable; }
   }
}
