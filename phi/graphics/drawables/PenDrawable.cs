using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.other;

namespace phi.graphics.drawables
{
   public abstract class PenDrawable : Drawable
   {
      private static readonly Color DEFAULT_COLOR = Color.Black;

      private Pen pen;

      public PenDrawable(int posX, int posY, int width, int height)
         : base(posX, posY, width, height)
      {
         pen = new Pen(DEFAULT_COLOR);
      }

      public PenDrawable(other.Rectangle rect) : base(rect)
      {
         pen = new Pen(DEFAULT_COLOR);
      }

      public PenDrawable(System.Drawing.Rectangle rect) : base(rect)
      {
         pen = new Pen(DEFAULT_COLOR);
      }

      public virtual PenDrawable SetColor(Color color) { this.pen = new Pen(color); FlagChange(); return this; }
      public virtual PenDrawable SetPen(Pen pen) { this.pen = pen; FlagChange(); return this; }
      protected virtual Pen GetPen() { return pen; }
   }
}
