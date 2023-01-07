using phi.graphics.drawables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea.visibleElements
{
    public class SynapseArrow : PenDrawable 
    {
        private const float baseToLeftRightBasesLength = 10f;
        private const float tipToBaseLength = 16.18f;
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
            float inputSlopeLength = (float)Math.Sqrt(
                slopeDeltaX * slopeDeltaX + slopeDeltaY * slopeDeltaY);
            float dX = slopeDeltaX / inputSlopeLength;
            float dY = slopeDeltaY / inputSlopeLength;
            float baseX = GetX() + dX * tipToBaseLength;
            float baseY = GetY() + dY * tipToBaseLength;

            // TODO verify v
            leftBaseX = (int)(baseX + dX * baseToLeftRightBasesLength);
            leftBaseY = (int)(baseY - dY * baseToLeftRightBasesLength);
            rightBaseX = (int)(baseX - dX * baseToLeftRightBasesLength);
            rightBaseY = (int)(baseY + dY * baseToLeftRightBasesLength);
        }
    }
}
