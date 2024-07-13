using Einstein.config;
using Einstein.model;
using Einstein.ui.editarea;
using phi.graphics;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using phi.other;
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
        public static readonly Pen BASE_OUTLINE_PEN = new Pen(Color.Black, 1f);
        public const int CIRCLE_RADIUS = 16;
        public const int CIRCLE_DIAMETER = 2 * CIRCLE_RADIUS;
        public const int FONT_SIZE = 10;
        public const int TEXT_MIN_HEIGHT = 16;
        public static readonly Color FONT_COLOR = EinsteinConfig.COLOR_MODE.Text;
        public const int DEFAULT_X = 0;
        public const int DEFAULT_Y = 0;


        public BaseNeuron Neuron { get; protected set; }
        private Sprite icon;
        private bool descEditable;
        private Text descText;
        private SelectableEditableText descSET;
        private float circleCenterX;
        private float circleCenterY;

        public NeuronDrawable(NeuronDrawable neuron) : this(neuron.Neuron) { }
        public NeuronDrawable(BaseNeuron neuron) : this(neuron, DEFAULT_X, DEFAULT_Y) { }
        public NeuronDrawable(BaseNeuron neuron, int x, int y) : base(x, y, CIRCLE_DIAMETER, CIRCLE_DIAMETER)
        {
            Neuron = neuron;
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
            descText = new Text.TextBuilder(neuron.Description)
                .WithColor(new SolidBrush(FONT_COLOR))
                .WithFontSize(FONT_SIZE)
                .WithMinHeight(TEXT_MIN_HEIGHT)
                .Build();
            descSET = null;

            circleCenterX = x + CIRCLE_RADIUS;
            circleCenterY = y + CIRCLE_RADIUS;
        }

        // If calling this, then you MUST also call DisableEditingDescription when you're done with this drawable... or else!
        public void EnableEditingDescription(BaseBrain brain)
        {
            if (descEditable)
            {
                descSET.Uninitialize();
            }
            descEditable = true;
            descSET = new NeuronDescSET(new NeuronDescET(descText, brain, Neuron),
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);
            descSET.Initialize();
        }

        public void DisableEditingDescription()
        {
            if (!descEditable) { return; }
            descEditable = false;
            descSET.Uninitialize();
            descSET = null;
        }

        public void SetColorGroup(Color paintColor)
        {
            Neuron.ColorGroup = paintColor;
            FlagChange();
        }

        public int GetCircleCenterX() { return GetX() + CIRCLE_RADIUS; }
        public int GetCircleCenterY() { return GetY() + CIRCLE_RADIUS; }
        public virtual void SetCircleCenterX(int x) { SetX(x - CIRCLE_RADIUS); circleCenterX = x; }
        public virtual void SetCircleCenterY(int y) { SetY(y - CIRCLE_RADIUS); circleCenterY = y; }
        public virtual void SetCircleCenterXY(int x, int y) { SetCircleCenterX(x); SetCircleCenterY(y); }

        public float GetCircleCenterXfloat() { return circleCenterX; }
        public float GetCircleCenterYfloat() { return circleCenterY; }
        public virtual void SetCircleCenterX(float x) { circleCenterX = x; SetX((int)(x - CIRCLE_RADIUS)); }
        public virtual void SetCircleCenterY(float y) { circleCenterY = y; SetY((int)(y - CIRCLE_RADIUS)); }
        public virtual void SetCircleCenterXY(float x, float y) { SetCircleCenterX(x); SetCircleCenterY(y); }

        public virtual int GetDescWidth() { return descText.GetWidth(); }

        protected override void DrawAt(Graphics g, int x, int y)
        {
            g.FillEllipse(new Pen(Neuron.ColorGroup).Brush, x, y, CIRCLE_DIAMETER - 1f, CIRCLE_DIAMETER - 1f);
            g.DrawEllipse(BASE_OUTLINE_PEN, x, y, CIRCLE_DIAMETER - 1f, CIRCLE_DIAMETER - 1f);

            icon?.SetXY(x, y);
            icon?.Draw(g);

            descText.SetCenterX(x + CIRCLE_RADIUS);
            descText.SetY(y + CIRCLE_DIAMETER);
            descText.Draw(g);
        }

        private static string getIconFileName(BaseNeuron neuron)
        {
            return BASE_DIR + neuron.Type.ToString() + ".png";
        }

        public override void PutIn(DynamicContainer container)
        {
            base.PutIn(container);
            icon.PutIn(container);
            descText.PutIn(container);
        }

        public override void TakeOut(DynamicContainer container)
        {
            base.TakeOut(container);
            icon.TakeOut(container);
            descText.TakeOut(container);
        }

        public override string ToString()
        {
            return Neuron.Index + " " + Neuron.ToString() + ", " + base.ToString();
        }
    }
}
