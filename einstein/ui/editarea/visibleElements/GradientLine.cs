using phi.graphics.drawables;
using phi.phisics.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea.visibleElements
{
    public class GradientLine : Line
    {
        public Color StartColor { get; private set; }
        public Color EndColor { get; private set; }
        public int Width { get; private set; }

        public GradientLine(Edge e, Color startColor, Color endColor, int width) : base(e)
        {
            this.StartColor = startColor;
            this.EndColor = endColor;
            this.Width = width;
        }

        public GradientLine(int startX, int startY, int endX, int endY, Color startColor, Color endColor, int width) : base(startX, startY, endX, endY)
        {
            this.StartColor = startColor;
            this.EndColor = endColor;
            this.Width = width;
        }

        protected override void DrawAt(Graphics g, int x, int y)
        {
            int x2 = x + GetWidth();
            int y2 = y + GetHeight();
            // I'm salty about this... apparently an OutOfMemoryException is thrown when
            // a gradient brush is instantiated with both points being the same.
            // https://social.msdn.microsoft.com/Forums/en-US/4832853b-bbe5-4a4f-a64d-dc4d045ae87c/pathgradientbrush-and-the-outofmemory-exception?forum=csharpgeneral
            // In this case, we can just avoid drawing if that's the case
            if (x == x2 && y == y2) { return; }
            LinearGradientBrush brush = new LinearGradientBrush(
                new Point(x, y),
                new Point(x2, y2),
                StartColor,
                EndColor);
            base.SetPen(new Pen(brush, Width));
            g.DrawLine(base.GetPen(), x, y, x2, y2);
        }

        // hide unsupported parent members
        private new void SetColor(Color color) { }
        private new void SetPen(Pen pen) { }
        private new void GetPen() { }
    }
}
