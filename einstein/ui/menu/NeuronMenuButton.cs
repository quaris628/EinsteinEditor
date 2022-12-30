using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class NeuronMenuButton : SelectableButton
    {
        public const string UNSELECTED_IMAGE_PATH =
            EinsteinPhiConfig.RES_DIR + "ButtonBackground.png";
        public const string SELECTED_IMAGE_PATH =
            EinsteinPhiConfig.RES_DIR + "ButtonBackgroundSelected.png";
        public const int WIDTH = 128;
        public const int HEIGHT = 32;

        private ICollection<NeuronDrawable> neuronOptions;

        public NeuronMenuButton(
                ICollection<NeuronDrawable> neuronOptions,
                int x, int y,
                string text,
                Action onSelect,
                Action onDeselect) :
            base(
                new Button.ButtonBuilder(
                    new ImageWrapper(UNSELECTED_IMAGE_PATH), x, y)
                    .withText(text)
                    .withOnClick(onSelect),
                new Button.ButtonBuilder(
                    new ImageWrapper(SELECTED_IMAGE_PATH), x, y)
                    .withText(text)
                    .withOnClick(onDeselect))
        {
            this.neuronOptions = neuronOptions;
        }

        public override void Initialize()
        {
            base.Initialize();
            IO.RENDERER.Add(this);
            foreach (NeuronDrawable neuron in neuronOptions)
            {
                IO.RENDERER.Add(neuron);
            }
            RepositionOptions();
            HideOptions();
        }

        public ICollection<NeuronDrawable> GetNeuronOptions()
        {
            return neuronOptions;
        }

        public void HideOptions()
        {
            foreach (NeuronDrawable neuron in neuronOptions)
            {
                neuron.SetDisplaying(false);
            }
        }

        public void ShowOptions()
        {
            foreach (NeuronDrawable neuron in neuronOptions)
            {
                neuron.SetDisplaying(true);
            }
        }

        // places buttons left to right on the screen, wrapping around if any would
        // overflow across the right edge of the screen, just like how english reads.
        public void RepositionOptions()
        {
            int startX = GetX() + GetWidth() + EinsteinPhiConfig.PAD;
            int x = startX;
            int y = EinsteinPhiConfig.PAD;
            // assumes all neuron option buttons are the same height
            // idk why the *2 is needed, but it's too cramped (vertically) otherwise
            int deltaY = NeuronDrawable.HEIGHT + EinsteinPhiConfig.PAD * 2;
            foreach (NeuronDrawable button in neuronOptions)
            {
                button.SetXY(x + button.GetWidth() / 2, y + NeuronDrawable.CIRCLE_RADIUS);
                x += (button.GetWidth() + EinsteinPhiConfig.PAD);
                if (x + button.GetWidth() + EinsteinPhiConfig.PAD > IO.WINDOW.GetWidth())
                {
                    x = startX;
                    y += deltaY;
                }
            }
        }
    }
}
