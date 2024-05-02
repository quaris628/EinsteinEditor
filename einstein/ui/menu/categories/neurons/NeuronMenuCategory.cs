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
    public class NeuronMenuCategory : MenuCategory
    {
        public MenuCategoryButton NeuronButton {
            get { return (MenuCategoryButton)Button; }
        }

        public NeuronMenuCategory(MenuCategoryButton button,
            IEnumerable<BaseNeuron> neuronOptions): base(button)
        {
            foreach (BaseNeuron neuron in neuronOptions)
            {
                sortedOptionDrawables.Add(neuron.Index, new NeuronDrawable(neuron));
            }

        }

        public IEnumerable<NeuronDrawable> GetNeuronDrawables()
        {
            foreach (Drawable optionDrawable in sortedOptionDrawables.Values)
            {
                yield return (NeuronDrawable)optionDrawable;
            }
        }

        // places options left to right on the screen, wrapping
        // around if any would overflow across the right edge
        // of the screen, just like the words in this comment.
        public override void RepositionOptions()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            int startX = NeuronButton.GetX() + NeuronButton.GetWidth() + EinsteinConfig.PAD;
            int x = startX;
            int y = EinsteinConfig.PAD;
            // assumes all neuron option buttons are the same height
            int deltaY = NeuronDrawable.CIRCLE_DIAMETER + NeuronDrawable.FONT_SIZE + EinsteinConfig.PAD * 2;
            foreach (NeuronDrawable optionDrawable in sortedOptionDrawables.Values)
            {
                int descWidth = optionDrawable.GetDescWidth();
                if (x + descWidth + EinsteinConfig.PAD > IO.WINDOW.GetWidth())
                {
                    // wrap around
                    x = startX;
                    y += deltaY;
                }
                optionDrawable.SetCircleCenterXY(x + descWidth / 2, y + NeuronDrawable.CIRCLE_RADIUS);
                x += descWidth + EinsteinConfig.PAD;
            }
            background.SetHeight(y + deltaY);
            background.SetWidth(IO.WINDOW.GetWidth() - startX);
        }

    }
}
