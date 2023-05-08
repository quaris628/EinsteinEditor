using Einstein.model;
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

namespace Einstein.ui.menu
{
    public class NeuronMenuCategory : MultiRenderable
    {
        public const int OPTION_LAYER = 15;
        public const int BACKGROUND_LAYER = 14;

        public NeuronMenuButton Button { get; protected set; }
        protected SortedDictionary<int, NeuronDrawable> neuronDrawables;
        protected RectangleDrawable background;
        protected bool isInit { get; private set; }

        public NeuronMenuCategory(NeuronMenuButton button,
            ICollection<BaseNeuron> neuronOptions)
        {
            Button = button;

            neuronDrawables = new SortedDictionary<int, NeuronDrawable>();
            foreach (BaseNeuron neuron in neuronOptions)
            {
                NeuronDrawable neuronDrawable = new NeuronDrawable(neuron);
                neuronDrawables.Add(neuron.Index, neuronDrawable);
            }

            // width and height will be set later
            background = new RectangleDrawable(NeuronMenuButton.WIDTH + 2 * EinsteinConfig.PAD, 0, 0, 0);
            background.SetPen(new Pen(new SolidBrush(EinsteinConfig.COLOR_MODE.MenuBackground)));
        }

        public virtual void Initialize()
        {
            Button.Initialize();
            Button.SubscribeOnSelect(ShowOptions);
            Button.SubscribeOnDeselect(HideOptions);
            IO.RENDERER.Add(Button);
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                IO.RENDERER.Add(neuronDrawable, OPTION_LAYER);
            }
            IO.RENDERER.Add(background, BACKGROUND_LAYER);
            isInit = true;
            RepositionOptions();
            HideOptions();
        }

        public virtual void Uninitialize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            isInit = false;
            Button.Uninitialize();
            IO.RENDERER.Remove(Button);
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                IO.RENDERER.Remove(neuronDrawable);
            }
            IO.RENDERER.Remove(background);
        }

        protected virtual void HideOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            background.SetDisplaying(false);
            foreach (NeuronDrawable neuron in neuronDrawables.Values)
            {
                neuron.SetDisplaying(false);
            }
        }

        protected virtual void ShowOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            background.SetDisplaying(true);
            foreach (NeuronDrawable neuron in neuronDrawables.Values)
            {
                neuron.SetDisplaying(true);
            }
            RepositionOptions();
        }

        // places buttons left to right on the screen, wrapping
        // around if any would overflow across the right edge
        // of the screen, just like the words in this comment.
        public virtual void RepositionOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            int startX = Button.GetX() + Button.GetWidth() + EinsteinConfig.PAD;
            int x = startX;
            int y = EinsteinConfig.PAD;
            // assumes all neuron option buttons are the same height
            int deltaY = NeuronDrawable.CIRCLE_DIAMETER + NeuronDrawable.FONT_SIZE + EinsteinConfig.PAD * 2;
            foreach (NeuronDrawable button in neuronDrawables.Values)
            {
                int descWidth = button.GetDescWidth();
                if (x + descWidth + EinsteinConfig.PAD > IO.WINDOW.GetWidth())
                {
                    x = startX;
                    y += deltaY;
                }
                button.SetCircleCenterXY(x + descWidth / 2, y + NeuronDrawable.CIRCLE_RADIUS);
                x += descWidth + EinsteinConfig.PAD;
            }
            background.SetHeight(y + deltaY);
            background.SetWidth(IO.WINDOW.GetWidth() - startX);
        }

        public virtual IEnumerable<Drawable> GetDrawables()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            yield return Button;
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                yield return neuronDrawable;
            }
            yield return background;
        }

        public virtual string LogDetailsForCrash()
        {
            string log = "Button.IsSelected() = " + Button.IsSelected();
            log += "\nneuronDrawables = " + string.Join(",\n\t", neuronDrawables);
            log += "\nisInit = " + isInit;
            return log;
        }
    }
}
