using Einstein.config;
using phi.graphics;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class LineArrow : MultiRenderable
    {
        public const int LINE_WIDTH = 2 * HALF_LINE_WIDTH;
        public const int HALF_LINE_WIDTH = 3;

        protected GradientLine line;
        protected SynapseArrow arrow;
        protected bool isInit { get; private set; }

        public LineArrow(int startX, int startY, int pointX, int pointY)
        {
            line = new GradientLine(startX, startY, pointX, pointY,
                EinsteinConfig.COLOR_MODE.SynapseBase,
                EinsteinConfig.COLOR_MODE.SynapseTip, LINE_WIDTH);
            arrow = new SynapseArrow(pointX, pointY, pointX - startX, pointY - startY);
            arrow.SetPen(new Pen(EinsteinConfig.COLOR_MODE.SynapseTip, LINE_WIDTH));
        }

        public virtual void Initialize(int layer)
        {
            IO.RENDERER.Add(line, layer);
            IO.RENDERER.Add(arrow, layer);
            isInit = true;
        }
        public virtual void Uninitialize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            isInit = false;
            IO.RENDERER.Remove(line);
            IO.RENDERER.Remove(arrow);
        }

        protected virtual void UpdateBaseXY(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            int oldX2 = line.GetX2();
            int oldY2 = line.GetY2();
            line.SetXY(x, y);
            line.SetXY2(oldX2, oldY2);
            arrow.SetDirection(line.GetX2() - x, line.GetY2() - y);
        }

        protected virtual void UpdateTipXY(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            line.SetXY2(x, y);
            arrow.SetXY(x, y);
            arrow.SetDirection(x - line.GetX(), y - line.GetY());
        }

        public virtual bool ContainsClick(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            // if the distance is less than half of the line's width,
            // or if the click is inside the triangle of the arrow
            return line.CalcSqDistanceToLine(x, y) <= HALF_LINE_WIDTH * HALF_LINE_WIDTH
                || arrow.TriangleContainsPoint(x, y);
        }

        public virtual IEnumerable<Drawable> GetDrawables()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            yield return line;
            yield return arrow;
        }

        public override string ToString()
        {
            return "Arrow" + line.ToString();
        }
    }
}
