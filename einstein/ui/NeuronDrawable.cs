using Einstein.model;
using phi.graphics;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui
{
    public class NeuronDrawable : Drawable
    {
        private const string BASE_DIR = EinsteinPhiConfig.RES_DIR + "neurons/";
        public const string BASE_IMAGE = BASE_DIR + "NeuronBase.png";
        public const int RADIUS_SIZE = 32;

        public BaseNeuron Neuron { get; protected set; }
        private Sprite baseSprite;
        private Sprite icon;

        public NeuronDrawable(BaseNeuron neuron, int x, int y) :
            base(x - RADIUS_SIZE, y - RADIUS_SIZE, RADIUS_SIZE * 2, RADIUS_SIZE * 2)
        {
            Neuron = neuron;
            baseSprite = new Sprite(new ImageWrapper(BASE_IMAGE), x - RADIUS_SIZE, y - RADIUS_SIZE);
            try
            {
                icon = new Sprite(new ImageWrapper(getIconFileName(neuron)), x - RADIUS_SIZE, y - RADIUS_SIZE);
            }
            // in case this neuron gets caught in the crossfire of changes to which neuron indices are output
            catch (InputOutputConflictException)
            {
                // don't show any special icon
            }

        }
        
        protected override void DrawAt(Graphics g, int x, int y)
        {
            baseSprite.SetCenterXY(x, y);
            icon?.SetCenterXY(x, y);
            baseSprite.Draw(g);
            icon?.Draw(g);
        }

        private static string getIconFileName(BaseNeuron neuron)
        {
            string midName;
            if (neuron.IsInput())
            {
                midName = "input/" + neuron.Description.ToString();
            }
            else if (neuron.IsOutput())
            {
                midName = "output/" + neuron.Description.ToString();
            }
            else
            {
                midName = "hidden/" + neuron.Type.ToString();
            }
            return BASE_DIR + midName + ".png";
        }
    }
}
