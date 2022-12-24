using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi;
using phi.phisics.Shapes;
using phi.graphics;
using phi.graphics.drawables;
using phi.io;

namespace Einstein
{
    class Ball : Circle
    {
        private const string IMAGE = EinsteinPhiConfig.RES_DIR + "Ball.png";

        private Sprite s;

        public Ball(double originX, double originY) : base(new Sprite(new ImageWrapper(IMAGE), (int)originX, (int)originY), originX, originY)
        {
            s = (Sprite)GetDrawable(); 
        }

    }
}
