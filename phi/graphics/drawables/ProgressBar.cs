using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phi.graphics.drawables
{
   public class ProgressBar : Drawable
   {
      protected double max, min, current;
      public ProgressBar(int x, int y, int width, int height, double max, double min, double current) : base(x, y, width, height)
      {
         this.min = min;
         this.max = max;
         this.current = current;
      }
      public ProgressBar(int x, int y, int width, int height, double max) : base(x, y, width, height)
      {
         this.min = 0;
         this.max = max;
         this.current = max;
      }
      protected override void DrawAt(Graphics g, int x, int y)
      {
         g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(x, y, (int)(width * (min + current) / max), height));
      }

      public double getCurrentValue()
      {
         return current;
      }

      public double getCurrentProgress()
      {
         return (min + current) / max;
      }

      public virtual bool SetToMax()
      {
         current = max;
         return true;
      }
      public virtual bool SetToMin()
      {
         current = min;
         return true;
      }
      public virtual bool SetProgress(double prog)
      {
         if (prog >= min && prog <= max)
         {
            current = prog;
            return true;
         }
         return false;
      }

      public virtual bool AddProgress()
      {
         if(current < max)
         {
            current++;
            return true;
         }
         return false;
      }
      public virtual bool AddProgress(double prog)
      {
         if(current + prog <= max && current + prog >= min)
         {
            current += prog;
            return true;
         }
         else if (current + prog > max)
         {
            current = max;
            return true;
         }
         else if (current + prog < min)
         {
            current = min;
            return true;
         }
         return false;
      }

      public virtual bool RemoveProgress()
      {
         if (current > min)
         {
            current--;
            return true;
         }
         else if (current - 1 <= min)
         {
            current = min;
            return true;
         }
         return false;
      }
      public virtual bool RemoveProgress(double prog)
      {
         if (current - prog <= max && current - prog >= min)
         {
            current -= prog;
            return true;
         }
         else if(current - prog > max)
         {
            current = max;
            return true;
         }
         else if(current - prog < min)
         {
            current = min;
            return true;
         }
         return false;
      }
   }
}
