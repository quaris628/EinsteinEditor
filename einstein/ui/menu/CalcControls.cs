using Einstein.config;
using Einstein.ui.editarea;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static phi.graphics.drawables.Text;

namespace Einstein.ui.menu
{
    public class CalcControls
    {
        private EditArea editArea;

        private SelectableButton showValuesToggle;
        private Button calcButton;
        private Text calcMsg;

        public CalcControls(EditArea editArea, int x, int y)
        {
            this.editArea = editArea;
            showValuesToggle = new SelectableButton(new Button.ButtonBuilder(
                    new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                    x,
                    y)
                    .withText("Show Values")
                    .withOnClick(showValues),
                    new Button.ButtonBuilder(
                    new ImageWrapper(MenuCategoryButton.SELECTED_IMAGE_PATH),
                    x,
                    y)
                    .withText("Hide Values")
                    .withOnClick(hideValues));
            calcButton = new Button.ButtonBuilder(
                    new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                    x,
                    EinsteinConfig.PAD + showValuesToggle.GetY() + showValuesToggle.GetHeight())
                    .withText("Calculate >")
                    .withOnClick(calcNeuronValues)
                    .Build();
            calcButton.SetDisplaying(showValuesToggle.IsSelected());

            calcMsg = new TextBuilder("Assumes sim speed = 1x")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(x,
                    EinsteinConfig.PAD + calcButton.GetY() + calcButton.GetHeight())
                .Build();
            calcMsg.SetDisplaying(showValuesToggle.IsSelected());
        }

        public void Initialize()
        {
            showValuesToggle.Initialize();
            IO.RENDERER.Add(showValuesToggle);
            calcButton.Initialize();
            IO.RENDERER.Add(calcButton);
            IO.RENDERER.Add(calcMsg);

            IO.KEYS.Subscribe(calcNeuronValues, EinsteinConfig.Keybinds.CALCULATE);
        }

        public void Uninitialize()
        {
            showValuesToggle.Uninitialize();
            IO.RENDERER.Remove(showValuesToggle);
            calcButton.Uninitialize();
            IO.RENDERER.Remove(calcButton);
            IO.RENDERER.Remove(calcMsg);

            IO.KEYS.Unsubscribe(calcNeuronValues, EinsteinConfig.Keybinds.CALCULATE);
        }

        private void showValues()
        {
            editArea.SetValuesDisplaying(true);
            calcButton.SetDisplaying(true);
            calcMsg.SetDisplaying(true);
        }

        private void hideValues()
        {
            editArea.SetValuesDisplaying(false);
            calcButton.SetDisplaying(false);
            calcMsg.SetDisplaying(false);
        }

        private void calcNeuronValues()
        {
            if (calcButton.IsDisplaying())
            {
                editArea.RefreshValuesText(NeuronValueCalculator.Calc(editArea.Brain));
            }
        }

    }
}
