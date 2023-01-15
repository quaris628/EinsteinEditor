using Einstein.model;
using phi.graphics;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class NeuronMenuCategory : MultiRenderable
    {
        public NeuronMenuButton Button { get; protected set; }
        protected SortedDictionary<int, NeuronDrawable> neuronDrawables;

        public NeuronMenuCategory(NeuronMenuButton button,
            ICollection<BaseNeuron> neuronOptions)
        {
            Button = button;
            button.SubscribeOnSelect(ShowOptions);
            button.SubscribeOnDeselect(HideOptions);

            neuronDrawables = new SortedDictionary<int, NeuronDrawable>();
            foreach (BaseNeuron neuron in neuronOptions)
            {
                NeuronDrawable neuronDrawable = new NeuronDrawable(neuron);
                neuronDrawables.Add(neuron.Index, neuronDrawable);
            }
        }

        public virtual void Initialize()
        {
            Button.Initialize();
            IO.RENDERER.Add(GetDrawables());
            RepositionOptions();
            HideOptions();
        }

        protected virtual void HideOptions()
        {
            foreach (NeuronDrawable neuron in neuronDrawables.Values)
            {
                neuron.SetDisplaying(false);
            }
        }

        protected virtual void ShowOptions()
        {
            foreach (NeuronDrawable neuron in neuronDrawables.Values)
            {
                neuron.SetDisplaying(true);
            }
        }

        // places buttons left to right on the screen, wrapping
        // around if any would overflow across the right edge
        // of the screen, just like the words in this comment.
        public virtual void RepositionOptions()
        {
            int startX = Button.GetX() + Button.GetWidth() + EinsteinPhiConfig.PAD;
            int x = startX;
            int y = EinsteinPhiConfig.PAD;
            // assumes all neuron option buttons are the same height
            int deltaY = NeuronDrawable.CIRCLE_DIAMETER + NeuronDrawable.FONT_SIZE + EinsteinPhiConfig.PAD * 2;
            foreach (NeuronDrawable button in neuronDrawables.Values)
            {
                int descWidth = button.GetDescriptionText().GetWidth();
                if (x + descWidth + EinsteinPhiConfig.PAD > IO.WINDOW.GetWidth())
                {
                    x = startX;
                    y += deltaY;
                }
                button.SetCircleCenterXY(x + descWidth / 2, y + NeuronDrawable.CIRCLE_RADIUS);
                x += descWidth + EinsteinPhiConfig.PAD;
            }
        }

        public virtual IEnumerable<Drawable> GetDrawables()
        {
            yield return Button;
            foreach (NeuronDrawable neuronDrawable in neuronDrawables.Values)
            {
                yield return neuronDrawable;
            }
        }

        public virtual string LogDetailsForCrash()
        {
            string log = "Button.IsSelected() = " + Button.IsSelected();
            log += "\nneuronDrawables = " + string.Join(",\n\t", neuronDrawables);
            return log;
        }
    }
}
