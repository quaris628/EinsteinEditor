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
            LinearGradientBrush brush = new LinearGradientBrush(
                new Point(x, y),
                new Point(x2, y2),
                StartColor,
                EndColor);
            g.DrawLine(new Pen(brush, Width), x, y, x2, y2);
        }

        // hide unsupported parent members
        private new void SetColor(Color color) { }
        private new void SetPen(Pen pen) { }
        private new void GetPen() { }
    }
}
