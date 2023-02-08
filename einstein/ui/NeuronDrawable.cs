using Einstein.config;
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
        private const string BASE_DIR = EinsteinConfig.RES_DIR + "neurons/";
        public static readonly string BASE_IMAGE = BASE_DIR + EinsteinConfig.COLOR_MODE.NeuronBaseFile;
        public const int CIRCLE_RADIUS = 16;
        public const int CIRCLE_DIAMETER = 2 * CIRCLE_RADIUS;
        public const int FONT_SIZE = 10;
        public static readonly Color FONT_COLOR = EinsteinConfig.COLOR_MODE.Text;
        public const int DEFAULT_X = 0;
        public const int DEFAULT_Y = 0;


        public BaseNeuron Neuron { get; protected set; }
        private Sprite baseSprite;
        private Sprite icon;
        private Text desc;

        public NeuronDrawable(int index, NeuronType type) :
            this(new BaseNeuron(index, type), DEFAULT_X, DEFAULT_Y) { }
        public NeuronDrawable(int index, NeuronType type, string description) :
            this(new BaseNeuron(index, type, description), DEFAULT_X, DEFAULT_Y) { }
        public NeuronDrawable(NeuronDrawable neuron) : this(neuron.Neuron) { }
        public NeuronDrawable(BaseNeuron neuron) : this(neuron, DEFAULT_X, DEFAULT_Y) { }
        public NeuronDrawable(BaseNeuron neuron, int x, int y) : base(x, y, CIRCLE_DIAMETER, CIRCLE_DIAMETER)
        {
            Neuron = neuron;
            baseSprite = new Sprite(new ImageWrapper(BASE_IMAGE), x, y);
            try
            {
                icon = new Sprite(new ImageWrapper(getIconFileName(neuron)), x, y);
            }
            // in case this neuron gets caught in the crossfire of changes
            // to which neuron indices are output
            catch (InputOutputAmbiguityException)
            {
                // don't show any special icon
            }
            desc = new Text.TextBuilder(neuron.Description)
                .WithColor(new SolidBrush(FONT_COLOR))
                .WithFontSize(FONT_SIZE).Build();
        }

        public Text GetDescriptionText() { return desc; }

        public int GetCircleCenterX() { return GetX() + CIRCLE_RADIUS; }
        public int GetCircleCenterY() { return GetY() + CIRCLE_RADIUS; }
        public virtual void SetCircleCenterX(int x) { SetX(x - CIRCLE_RADIUS); }
        public virtual void SetCircleCenterY(int y) { SetY(y - CIRCLE_RADIUS); }
        public virtual void SetCircleCenterXY(int x, int y) { SetXY(x - CIRCLE_RADIUS, y - CIRCLE_RADIUS); }

        protected override void DrawAt(Graphics g, int x, int y)
        {
            baseSprite.SetXY(x, y);
            baseSprite.Draw(g);

            icon?.SetXY(x, y);
            icon?.Draw(g);

            desc.SetCenterX(x + CIRCLE_RADIUS);
            desc.SetY(y + CIRCLE_DIAMETER);
            desc.Draw(g);
        }

        private static string getIconFileName(BaseNeuron neuron)
        {
            return BASE_DIR + neuron.Type.ToString() + ".png";
        }

        public override string ToString()
        {
            return Neuron.Index + " " + Neuron.ToString() + ", " + base.ToString();
        }
    }
}
