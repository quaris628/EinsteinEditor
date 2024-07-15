using Einstein.config;
using Einstein.config.bibiteVersions;
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
using static phi.graphics.renderables.IntET;

namespace Einstein.ui.menu
{
    public class CalcControls
    {
        private const float SIM_SPEED_MIN = 0.09f;
        private const float SIM_SPEED_MAX = 11.18f;
        private const int SIM_SPEED_MAX_DECIMALS = 2;
        private const string DEFAULT_SIM_SPEED = "1";

        private const int TICKS_PER_SECOND_MIN = 5;
        private const int TICKS_PER_SECOND_MAX = 100;
        private const string DEFAULT_TICKS_PER_SECOND = "40";

        private const int BRAIN_UPDATE_FACTOR_MIN = 1;
        private const int BRAIN_UPDATE_FACTOR_MAX = 100;
        private const string DEFAULT_BRAIN_UPDATE_FACTOR = "2";

        private EditArea editArea;
        private float deltaTimePerTick;

        private SelectableButton showValuesToggle;
        private Button calcButton;
        
        private Text simSpeedLabel;
        private Text simSpeedText;
        private SelectableEditableText simSpeedSET;
        private Text assumeFpsMsg;

        private Text ticksPerSecondLabel;
        private Text ticksPerSecondText;
        private SelectableEditableText ticksPerSecondSET;
        private Text brainUpdateFactorLabel;
        private Text brainUpdateFactorText;
        private SelectableEditableText brainUpdateFactorSET;

        private Text deltaTimeMsg;

        public CalcControls(EditArea editArea, int x, int y)
        {
            this.editArea = editArea;
            deltaTimePerTick = getDeltaTimeFromSimSpeed(DEFAULT_SIM_SPEED);

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
            simSpeedText = new TextBuilder(DEFAULT_SIM_SPEED)
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    .WithXY(simSpeedLabel.GetX() + 105, simSpeedLabel.GetY())
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
            assumeFpsMsg = new TextBuilder("(Assuming 60 fps)")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(x,
                    EinsteinConfig.PAD + simSpeedText.GetY() + simSpeedText.GetHeight())
                .Build();

            ticksPerSecondLabel = new TextBuilder("Ticks per Second:")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(11f)
                .WithXY(x,
                    EinsteinConfig.PAD + calcButton.GetY() + calcButton.GetHeight())
                .Build();
            ticksPerSecondText = new TextBuilder(DEFAULT_TICKS_PER_SECOND)
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    .WithFontSize(11f)
                    .WithXY(ticksPerSecondLabel.GetX() + 130, ticksPerSecondLabel.GetY())
                    .Build();
            ticksPerSecondSET = new SelectableEditableText(
                ((IntETBuilder)new IntETBuilder(ticksPerSecondText)
                    .WithEditingDisabled()
                    .WithMinValue(TICKS_PER_SECOND_MIN)
                    .WithMaxValue(TICKS_PER_SECOND_MAX)
                    .WithAnchor(ticksPerSecondLabel.GetX() + 130, ticksPerSecondLabel.GetY())
                    .WithOnEdit(onEditTicksPerSecond)
                    ).Build(),
                DEFAULT_TICKS_PER_SECOND,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);

            brainUpdateFactorLabel = new TextBuilder("Brain Update Factor:")
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(11f)
                .WithXY(x,
                    EinsteinConfig.PAD + ticksPerSecondLabel.GetY() + ticksPerSecondLabel.GetHeight())
                .Build();
            brainUpdateFactorText = new TextBuilder(DEFAULT_BRAIN_UPDATE_FACTOR)
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                    .WithFontSize(11f)
                    .WithXY(brainUpdateFactorLabel.GetX() + 136, brainUpdateFactorLabel.GetY())
                    .Build();
            brainUpdateFactorSET = new SelectableEditableText(
                ((IntETBuilder)new IntETBuilder(brainUpdateFactorText)
                    .WithEditingDisabled()
                    .WithMinValue(BRAIN_UPDATE_FACTOR_MIN)
                    .WithMaxValue(BRAIN_UPDATE_FACTOR_MAX)
                    .WithAnchor(brainUpdateFactorLabel.GetX() + 136, brainUpdateFactorLabel.GetY())
                    .WithOnEdit(onEditBrainUpdateFactor)
                    ).Build(),
                DEFAULT_BRAIN_UPDATE_FACTOR,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundSelected,
                EinsteinConfig.COLOR_MODE.EditableTextBackgroundUnselected);

            deltaTimeMsg = new TextBuilder(getDeltaTimeMsg())
                    .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                    .WithXY(x,
                        EinsteinConfig.PAD + assumeFpsMsg.GetY() + assumeFpsMsg.GetHeight())
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
            IO.RENDERER.Add(assumeFpsMsg);

            IO.RENDERER.Add(ticksPerSecondLabel);
            IO.RENDERER.Add(ticksPerSecondText);
            ticksPerSecondSET.Initialize();
            IO.RENDERER.Add(brainUpdateFactorLabel);
            IO.RENDERER.Add(brainUpdateFactorText);
            brainUpdateFactorSET.Initialize();

            IO.RENDERER.Add(deltaTimeMsg);
            recalculateDeltaTimePerTick();
            deltaTimeMsg.SetMessage(getDeltaTimeMsg());
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
            IO.RENDERER.Remove(assumeFpsMsg);

            IO.RENDERER.Remove(ticksPerSecondLabel);
            IO.RENDERER.Remove(ticksPerSecondText);
            ticksPerSecondSET.Uninitialize();
            IO.RENDERER.Remove(brainUpdateFactorLabel);
            IO.RENDERER.Remove(brainUpdateFactorText);
            brainUpdateFactorSET.Uninitialize();

            IO.RENDERER.Remove(deltaTimeMsg);

            IO.KEYS.Unsubscribe(calcNeuronValues, EinsteinConfig.Keybinds.CALCULATE);
        }

        private void showValues()
        {
            editArea.SetValuesDisplaying(true);
            calcButton.SetDisplaying(true);
            if (editArea.BibiteVersion.GetDeltaTimeCalcMethod() == BibiteVersion.DeltaTimeCalcMethod.SimSpeed)
            {
                simSpeedLabel.SetDisplaying(true);
                simSpeedText.SetDisplaying(true);
                assumeFpsMsg.SetDisplaying(true);
            }
            else if (editArea.BibiteVersion.GetDeltaTimeCalcMethod() == BibiteVersion.DeltaTimeCalcMethod.BrainUpdateFactorOverTps)
            {
                ticksPerSecondLabel.SetDisplaying(true);
                ticksPerSecondText.SetDisplaying(true);
                brainUpdateFactorLabel.SetDisplaying(true);
                brainUpdateFactorText.SetDisplaying(true);
            }
            deltaTimeMsg.SetDisplaying(true);
            
        }

        private void hideValues()
        {
            editArea.SetValuesDisplaying(false);
            calcButton.SetDisplaying(false);

            simSpeedLabel.SetDisplaying(false);
            simSpeedText.SetDisplaying(false);
            assumeFpsMsg.SetDisplaying(false);

            ticksPerSecondLabel.SetDisplaying(false);
            ticksPerSecondText.SetDisplaying(false);
            brainUpdateFactorLabel.SetDisplaying(false);
            brainUpdateFactorText.SetDisplaying(false);

            deltaTimeMsg.SetDisplaying(false);
        }

        public void OnBibiteVersionUpdate()
        {
            hideValues();
            recalculateDeltaTimePerTick();
            deltaTimeMsg.SetMessage(getDeltaTimeMsg());
            if (showValuesToggle.IsSelected())
            {
                showValues();
            }
        }

        private void calcNeuronValues()
        {
            if (calcButton.IsDisplaying())
            {
                editArea.RefreshValuesText(NeuronValueCalculator.Calc(editArea.Brain, deltaTimePerTick));
            }
        }

        private void recalculateDeltaTimePerTick()
        {
            if (editArea.BibiteVersion.GetDeltaTimeCalcMethod() == BibiteVersion.DeltaTimeCalcMethod.SimSpeed)
            {
                string simSpeed = simSpeedText.GetMessage();
                deltaTimePerTick = getDeltaTimeFromSimSpeed(simSpeed);
            }
            else if (editArea.BibiteVersion.GetDeltaTimeCalcMethod() == BibiteVersion.DeltaTimeCalcMethod.BrainUpdateFactorOverTps)
            {
                string ticksPerSecond = ticksPerSecondText.GetMessage();
                string brainUpdateFactor = brainUpdateFactorText.GetMessage();
                deltaTimePerTick = getDeltaTimeFromBufAndTps(brainUpdateFactor, ticksPerSecond);
            }
        }

        private void onEditSimSpeed(string simSpeed)
        {
            try
            {
                deltaTimePerTick = getDeltaTimeFromSimSpeed(simSpeed);
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

        private static float getDeltaTimeFromSimSpeed(string simSpeed)
        {
            // assume 60 fps
            return CustomNumberParser.StringToFloat(simSpeed) / 60;
        }

        private void onEditTicksPerSecond(string ticksPerSecond)
        {
            string brainUpdateFactor = brainUpdateFactorText.GetMessage();
            try
            {
                deltaTimePerTick = getDeltaTimeFromBufAndTps(brainUpdateFactor, ticksPerSecond);
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

        private void onEditBrainUpdateFactor(string brainUpdateFactor)
        {
            string ticksPerSecond = ticksPerSecondText.GetMessage();
            try
            {
                deltaTimePerTick = getDeltaTimeFromBufAndTps(brainUpdateFactor, ticksPerSecond);
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

        private static float getDeltaTimeFromBufAndTps(string brainUpdateFactor, string ticksPerSecond)
        {
            return (float)CustomNumberParser.StringToInt(brainUpdateFactor) / CustomNumberParser.StringToInt(ticksPerSecond);
        }

        private string getDeltaTimeMsg()
        {
            return $"{deltaTimePerTick:0.#####}s between ticks";
        }
    }
}
