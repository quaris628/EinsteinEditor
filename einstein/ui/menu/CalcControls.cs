using Einstein.config;
using Einstein.ui.editarea;
using LibraryFunctionReplacements;
using phi.graphics.drawables;
using phi.graphics.renderables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static phi.graphics.drawables.Text;
using static phi.graphics.renderables.EditableText;
using static phi.graphics.renderables.FloatET;

namespace Einstein.ui.menu
{
    public class CalcControls
    {
        private const float SIM_SPEED_MIN = 0.09f;
        private const float SIM_SPEED_MAX = 11.18f;
        private const int SIM_SPEED_MAX_DECIMALS = 2;
        private const string DEFAULT_SIM_SPEED = "1";

        private EditArea editArea;
        private float deltaTimePerTick;

        private SelectableButton showValuesToggle;
        private Button calcButton;
        private Text simSpeedLabel;
        private Text simSpeedText;
        private SelectableEditableText simSpeedSET;
        private Text deltaTimeMsg;
        private Text calcMsg;

        public CalcControls(EditArea editArea, int x, int y)
        {
            this.editArea = editArea;
            deltaTimePerTick = getDeltaTime(DEFAULT_SIM_SPEED);

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

            simSpeedLabel = new TextBuilder("Sim speed:")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithXY(x,
                    EinsteinConfig.PAD + calcButton.GetY() + calcButton.GetHeight())
                .Build();
            simSpeedText = new Text.TextBuilder(DEFAULT_SIM_SPEED)
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    //.WithBackgroundColor(new SolidBrush(EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected))
                    .WithXY(simSpeedLabel.GetX() + 40, simSpeedLabel.GetY())
                    .Build();
            simSpeedSET = new SelectableEditableText(
                ((FloatETBuilder)new FloatETBuilder(simSpeedText)
                    .WithEditingDisabled()
                    .WithMinValue(SIM_SPEED_MIN)
                    .WithMaxValue(SIM_SPEED_MAX)
                    .WithMaxDecimalPlaces(SIM_SPEED_MAX_DECIMALS)
                    .WithAnchor(simSpeedLabel.GetX() + 105, simSpeedLabel.GetY())
                    .WithOnEdit(onEditSimSpeed)
                    ).Build(),
                DEFAULT_SIM_SPEED,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);
            deltaTimeMsg = new Text.TextBuilder(getDeltaTimeMsg())
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                    .WithXY(x,
                        EinsteinConfig.PAD + simSpeedText.GetY() + simSpeedText.GetHeight())
                    .Build();

            calcMsg = new TextBuilder("(Assuming 60 fps)")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(x,
                    EinsteinConfig.PAD + deltaTimeMsg.GetY() + deltaTimeMsg.GetHeight())
                .Build();
        }

        public void Initialize()
        {
            showValuesToggle.Initialize();
            IO.RENDERER.Add(showValuesToggle);
            IO.RENDERER.Add(calcButton);
            calcButton.Initialize();
            IO.RENDERER.Add(simSpeedLabel);
            IO.RENDERER.Add(simSpeedText);
            simSpeedSET.Initialize();
            IO.RENDERER.Add(deltaTimeMsg);
            IO.RENDERER.Add(calcMsg);
            hideValues();

            IO.KEYS.Subscribe(calcNeuronValues, EinsteinConfig.Keybinds.CALCULATE);
        }

        public void Uninitialize()
        {
            showValuesToggle.Uninitialize();
            IO.RENDERER.Remove(showValuesToggle);
            IO.RENDERER.Remove(calcButton);
            calcButton.Uninitialize();
            IO.RENDERER.Remove(simSpeedLabel);
            IO.RENDERER.Remove(simSpeedText);
            simSpeedSET.Uninitialize();
            IO.RENDERER.Remove(deltaTimeMsg);
            IO.RENDERER.Remove(calcMsg);

            IO.KEYS.Unsubscribe(calcNeuronValues, EinsteinConfig.Keybinds.CALCULATE);
        }

        private void showValues()
        {
            editArea.SetValuesDisplaying(true);
            calcButton.SetDisplaying(true);
            simSpeedLabel.SetDisplaying(true);
            simSpeedText.SetDisplaying(true);
            deltaTimeMsg.SetDisplaying(true);
            calcMsg.SetDisplaying(true);
        }

        private void hideValues()
        {
            editArea.SetValuesDisplaying(false);
            calcButton.SetDisplaying(false);
            simSpeedLabel.SetDisplaying(false);
            simSpeedText.SetDisplaying(false);
            deltaTimeMsg.SetDisplaying(false);
            calcMsg.SetDisplaying(false);
        }

        private void calcNeuronValues()
        {
            if (calcButton.IsDisplaying())
            {
                float simSpeed = CustomNumberParser.StringToFloat(simSpeedText.GetMessage());
                float deltaTime = simSpeed / 60; // assumes 60 fps
                editArea.RefreshValuesText(NeuronValueCalculator.Calc(editArea.Brain, deltaTime));
            }
        }

        private void onEditSimSpeed(string simSpeed)
        {
            try
            {
                // assume 60 fps
                deltaTimePerTick = getDeltaTime(simSpeed);
            }
            catch (ArithmeticException)
            {
                // keep last valid value
                return;
            }
            catch (ArgumentException)
            {
                // keep last valid value
                return;
            }

            deltaTimeMsg.SetMessage(getDeltaTimeMsg());
        }

        private float getDeltaTime(string simSpeed)
        {
            return CustomNumberParser.StringToFloat(simSpeed) / 60;
        }

        private string getDeltaTimeMsg()
        {
            return $"{deltaTimePerTick:0.#####}s between ticks";
        }
    }
}
