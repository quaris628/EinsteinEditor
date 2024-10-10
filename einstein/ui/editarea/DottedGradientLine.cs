using phi.graphics.drawables;
using phi.phisics.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class DottedGradientLine : Line, GenericGradientLine
    {
        private Color _startColor;
        public Color StartColor {
            get { return _startColor; }
            set
            {
                _startColor = value;
                FlagChange();
            }
        }
        private Color _endColor;
        public Color EndColor
        {
            get { return _endColor; }
            set
            {
                _endColor = value;
                FlagChange();
            }
        }
        public int Width { get; private set; }
        public int DotDistance { get; private set; }
        public int SpaceDistance { get; private set; }

        public DottedGradientLine(int startX, int startY, int endX, int endY,
            Color startColor, Color endColor,
            int width, int dotDistance, int spaceDistance)
            : base(startX, startY, endX, endY)
        {
            this.StartColor = startColor;
            this.EndColor = endColor;
            this.Width = width;
            this.DotDistance = dotDistance;
            this.SpaceDistance = spaceDistance;
        }

        public DottedGradientLine(int startX, int startY, int endX, int endY, Color startColor, Color endColor, int width) : base(startX, startY, endX, endY)
        {
            this.StartColor = startColor;
            this.EndColor = endColor;
            this.Width = width;
            this.DotDistance = width;
            this.SpaceDistance = width;
        }

        protected override void DrawAt(Graphics g, int startX, int startY)
        {
            int endX = startX + GetWidth();
            int endY = startY + GetHeight();
            // I'm salty about this... apparently an OutOfMemoryException is thrown when
            // a gradient brush is instantiated with both points being the same.
            // https://social.msdn.microsoft.com/Forums/en-US/4832853b-bbe5-4a4f-a64d-dc4d045ae87c/pathgradientbrush-and-the-outofmemory-exception?forum=csharpgeneral
            // In this case, we can just avoid drawing if that's the case
            if (startX == endX && startY == endY) { return; }

            // Calculate slope and x/y distances for dots
            float oneOverHypotenuse = 1 / (float)Math.Sqrt(GetWidth() * GetWidth() + GetHeight() * GetHeight());
            float cosAngle = GetWidth() * oneOverHypotenuse;
            float sinAngle = GetHeight() * oneOverHypotenuse;
            float xDotDelta = DotDistance * cosAngle;
            float yDotDelta = DotDistance * sinAngle;
            float xTotalStepDelta = (SpaceDistance + DotDistance) * cosAngle;
            float yTotalStepDelta = (SpaceDistance + DotDistance) * sinAngle;

            LinearGradientBrush brush = new LinearGradientBrush(
                new Point(startX, startY),
                new Point(endX, endY),
                StartColor,
                EndColor);
            base.SetPen(new Pen(brush, Width));

            float x2 = endX;
            float y2 = endY;
            // Go backwards, from tip to base
            while ((startX < endX ? startX <= x2 : x2 <= startX)
                && (startY < endY ? startY <= y2 : y2 <= startY))
            {
                float x1 = x2 - xDotDelta;
                float y1 = y2 - yDotDelta;
                g.DrawLine(base.GetPen(), x1, y1, x2, y2);
                x2 -= xTotalStepDelta;
                y2 -= yTotalStepDelta;
            }
        }

        // hide unsupported parent members
        private new void SetColor(Color color) { }
        private new void SetPen(Pen pen) { }
        private new void GetPen() { }
    }
}
