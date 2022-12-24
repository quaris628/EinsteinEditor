using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.other;

namespace phi.graphics.renderables
{
   // I don't remember if this class was fully finished.
   // Use / extend with caution. -Nathan
   public class Snappable : Draggable
   {
      private Dictionary<Rectangle, SnapToCommand> rectCatches;

      public Snappable(Drawable drawable) : base(drawable)
      {
         rectCatches = new Dictionary<Rectangle, SnapToCommand>();
      }

      public void AddSnapCatchArea(SnapToCommand snap, Rectangle rect)
      {
         rectCatches.Add(rect, snap);
      }

      protected override void MyMouseUp(int x, int y)
      {
         foreach (Rectangle rect in rectCatches.Keys)
         {
            if (rect.Contains(x, y))
            {
               rectCatches[rect].Execute(GetDrawable());
               return;
            }
         }
      }

      // sorta command pattern
      // different snaps snap differently
      public abstract class SnapToCommand
      {
         protected int x;
         protected int y;

         public SnapToCommand(int x, int y)
         {
            this.x = x;
            this.y = y;
         }

         public abstract void Execute(Drawable drawable);
      }

      public class SnapToTopLeftCommand : SnapToCommand
      {
         public SnapToTopLeftCommand(int x, int y) : base(x, y) { }
         public SnapToTopLeftCommand(Rectangle rect) : base(rect.X, rect.Y) { }

         public override void Execute(Drawable drawable)
         {
            drawable.SetXY(x, y);
         }
      }

      public class SnapToCenterCommand : SnapToCommand
      {
         public SnapToCenterCommand(int x, int y) : base(x, y) { }
         public SnapToCenterCommand(Rectangle rect)
            : base(rect.X + rect.Width / 2, rect.Y + rect.Height / 2) { }

         public override void Execute(Drawable drawable)
         {
            drawable.SetCenterXY(x, y);
         }
      }


   }
}
