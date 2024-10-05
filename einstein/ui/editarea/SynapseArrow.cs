using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class SynapseArrow : PenDrawable 
    {
        public const float HALF_WIDTH = 8f;
        public const float LENGTH = 16f;
        public readonly static float HYPOTENUSE_LENGTH =
            (float)Math.Sqrt(HALF_WIDTH * HALF_WIDTH + LENGTH * LENGTH);

        private int leftBaseX;
        private int leftBaseY;
        private int rightBaseX;
        private int rightBaseY;

        public SynapseArrow(int tipX, int tipY, float slopeDeltaX, float slopeDeltaY)
            : base(tipX, tipY, 0, 0)
        {
            SetDirection(slopeDeltaX, slopeDeltaY);
        }

        protected override void DrawAt(Graphics g, int x, int y)
        {
            g.DrawLine(GetPen(), x, y, leftBaseX, leftBaseY);
            g.DrawLine(GetPen(), x, y, rightBaseX, rightBaseY);
        }

        public void SetDirection(float slopeDeltaX, float slopeDeltaY)
        {
            if (slopeDeltaX == 0 && slopeDeltaY == 0)
            {
                // direction isn't defined, so just keep previous direction
                return;
            }
            float inputSlopeLength = (float)Math.Sqrt(
                slopeDeltaX * slopeDeltaX + slopeDeltaY * slopeDeltaY);
            float dX = -slopeDeltaX / inputSlopeLength;
            float dY = -slopeDeltaY / inputSlopeLength;
            float baseX = GetX() + dX * LENGTH;
            float baseY = GetY() + dY * LENGTH;

            leftBaseX = (int)(baseX + dY * HALF_WIDTH);
            leftBaseY = (int)(baseY - dX * HALF_WIDTH);
            rightBaseX = (int)(baseX - dY * HALF_WIDTH);
            rightBaseY = (int)(baseY + dX * HALF_WIDTH);
        }

        public bool TriangleContainsPoint(int x, int y)
        {
            // quick check first for being inside square around tip point
            int taxicabDistance = Math.Abs(x - GetX()) + Math.Abs(y - GetY());
            if (taxicabDistance > HYPOTENUSE_LENGTH * 2)
            {
                return false;
            }

            // slower, (more) exact check second

            // Actual click area needs to include the width of the line being drawn (at least roughly)
            float extraLineWidth = GetPen().Width * 0.5f;
            float cosBaseAngle = (rightBaseX - leftBaseX) / (2 * HALF_WIDTH);
            float sinBaseAngle = (rightBaseY - leftBaseY) / (2 * HALF_WIDTH);
            int clickAreaLeftBaseX = (int)(leftBaseX - cosBaseAngle * extraLineWidth);
            int clickAreaLeftBaseY = (int)(leftBaseY - sinBaseAngle * extraLineWidth);
            int clickAreaRightBaseX = (int)(rightBaseX + cosBaseAngle * extraLineWidth);
            int clickAreaRightBaseY = (int)(rightBaseY + sinBaseAngle * extraLineWidth);
            int clickAreaTipX = (int)(GetX() - sinBaseAngle * extraLineWidth);
            int clickAreaTipY = (int)(GetY() + cosBaseAngle * extraLineWidth);

            // this way seems roundabout, but I think it's the easiest:
            // calculate the areas of the 3 triangles the point would divide the
            // arrow triangle into if the point was in the interior
            int area1 = triangle2xArea(x, y, clickAreaLeftBaseX, clickAreaLeftBaseY, clickAreaRightBaseX, clickAreaRightBaseY);
            int area2 = triangle2xArea(clickAreaTipX, clickAreaTipY, x, y, clickAreaRightBaseX, clickAreaRightBaseY);
            int area3 = triangle2xArea(clickAreaTipX, clickAreaTipY, clickAreaLeftBaseX, clickAreaLeftBaseY, x, y);
            // if the sum of the areas is greater than the area of the whole triangle,
            // then the point can't be inside the arrow triangle.
            int bigArea = triangle2xArea(clickAreaTipX, clickAreaTipY, clickAreaLeftBaseX, clickAreaLeftBaseY, clickAreaRightBaseX, clickAreaRightBaseY);
            return area1 + area2 + area3 <= bigArea;
        }

        private int triangle2xArea(
            int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return Math.Abs(x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));
        }
    }
}
