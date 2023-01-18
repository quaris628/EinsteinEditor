using Einstein.model;
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
        public const int WIDTH = 160;
        public const int HEIGHT = 32;

        public NeuronMenuButton(
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
            
        }

    }
}
