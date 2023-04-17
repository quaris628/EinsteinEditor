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
      private int boundsType; // 0 = none, 1 = static, 2 = dynamic
      private Rectangle dragBounds;
      private Func<Rectangle> getDynamicDragBounds;
      private bool useRightMouseButton;
      protected bool isMoving { get; private set; }
      protected bool isInit;

      public Draggable(Drawable drawable)
         : this(drawable, new Rectangle(), DEFAULT_USE_RIGHT_MOUSE_BUTTON) { }
      public Draggable(Drawable drawable, bool useRightMouseButton)
      {
         this.drawable = drawable;
         this.boundsType = 0;
         this.useRightMouseButton = useRightMouseButton;
         this.isMoving = false;
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
         this.boundsType = 1;
         this.dragBounds = dragBounds;
         this.useRightMouseButton = useRightMouseButton;
         this.isMoving = false;
      }
      public Draggable(Drawable drawable, Func<Rectangle> getDynamicDragBounds)
         : this(drawable, getDynamicDragBounds, DEFAULT_USE_RIGHT_MOUSE_BUTTON) { }
      public Draggable(Drawable drawable, Func<Rectangle> getDynamicDragBounds, bool useRightMouseButton)
      {
         this.drawable = drawable;
         this.boundsType = 2;
         this.getDynamicDragBounds = getDynamicDragBounds;
         this.useRightMouseButton = useRightMouseButton;
         this.isMoving = false;
      }

      public virtual void Initialize()
      {
         if (useRightMouseButton)
         {
            IO.MOUSE.RIGHT_DOWN.SubscribeOnDrawable(MouseDown, drawable);
         }
         else
         {
            IO.MOUSE.LEFT_DOWN.SubscribeOnDrawable(MouseDown, drawable);
         }
         isInit = true;
      }
      
      public virtual void Uninitialize()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         isInit = false;
         if (isMoving)
         {
            IO.MOUSE.MOVE.Unsubscribe(MouseMove);
            if (useRightMouseButton)
            {
               IO.MOUSE.RIGHT_UP.Unsubscribe(MouseUp);
               IO.MOUSE.RIGHT_DOWN.UnsubscribeFromDrawable(MouseDown, drawable);
            }
            else
            {
               IO.MOUSE.LEFT_UP.Unsubscribe(MouseUp);
               IO.MOUSE.LEFT_DOWN.UnsubscribeFromDrawable(MouseDown, drawable);
            }
         }
         else
         {
            if (useRightMouseButton)
            {
               IO.MOUSE.RIGHT_DOWN.UnsubscribeFromDrawable(MouseDown, drawable);
            }
            else
            {
               IO.MOUSE.LEFT_DOWN.UnsubscribeFromDrawable(MouseDown, drawable);
            }
         }
         
      }

      private void MouseDown(int x, int y)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (!isMoving)
         {
            isMoving = true;
            IO.MOUSE.MOVE.Subscribe(MouseMove);
            if (useRightMouseButton)
            {
               IO.MOUSE.RIGHT_UP.Subscribe(MouseUp);
            }
            else
            {
               IO.MOUSE.LEFT_UP.Subscribe(MouseUp);
            }
         }
         dragOffsetX = x - drawable.GetX();
         dragOffsetY = y - drawable.GetY();
         MyMouseDown(x, y);
      }
      protected virtual void MyMouseDown(int x, int y) { }

      private void MouseMove(int x, int y)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (boundsType == 2)
         {
            dragBounds = getDynamicDragBounds.Invoke();
         }
         if (boundsType != 0)
         {
            drawable.SetX(Math.Max(dragBounds.X,
               Math.Min(x - dragOffsetX,
               dragBounds.X + dragBounds.Width - drawable.GetWidth())));
            drawable.SetY(Math.Max(dragBounds.Y,
               Math.Min(y - dragOffsetY,
               dragBounds.Y + dragBounds.Height - drawable.GetHeight())));
         }
         MyMouseMove(x, y);
      }
      protected virtual void MyMouseMove(int x, int y) { }

      private void MouseUp(int x, int y)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         if (isMoving)
         {
            isMoving = false;
            IO.MOUSE.MOVE.Unsubscribe(MouseMove);
            if (useRightMouseButton)
            {
               IO.MOUSE.RIGHT_UP.Unsubscribe(MouseUp);
            }
            else
            {
               IO.MOUSE.LEFT_UP.Unsubscribe(MouseUp);
            }
         }
         MyMouseUp(x, y);
      }
      protected virtual void MyMouseUp(int x, int y) { }

      protected int GetDragOffsetX()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return dragOffsetX;
      }
      protected int GetDragOffsetY()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return dragOffsetY;
      }

      public bool HasDragBounds()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         return this.boundsType != 0;
      }
      public void SetDragUnbounded()
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         this.boundsType = 0;
      }
      
      public void SetDragBounds(Rectangle bounds)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         this.boundsType = 1;
         this.dragBounds = bounds;
      }

      public void SetDragBounds(int minX, int minY, int maxX, int maxY)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         this.boundsType = 1;
         this.dragBounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
      }

      public void SetDragBounds(Func<Rectangle> getDynamicDragBounds)
      {
         if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
         this.boundsType = 2;
         this.getDynamicDragBounds = getDynamicDragBounds;
      }

      public Drawable GetDrawable()
      {
         return drawable;
      }
   }
}
