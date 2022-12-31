using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using phi.graphics;
using phi.control;
using phi.io;
using phi.graphics.renderables;
using phi.graphics.drawables;
using Einstein.model;
using Einstein.ui;
using Einstein.ui.menu;
using phi.other;

namespace Einstein
{
    class EditorScene : Scene
    {
        // ----------------------------------------------------------------
        //  Config
        // ----------------------------------------------------------------

        public const int SPAWN_X = 250 + NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD;
        public const int SPAWN_Y = 200 + (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3;
        public static readonly Rectangle BRAIN_AREA = new Rectangle(
            NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3,
            EinsteinPhiConfig.Window.WIDTH - NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            EinsteinPhiConfig.Window.HEIGHT - (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3);

        // ----------------------------------------------------------------
        //  Data/Constructor
        // ----------------------------------------------------------------

        private BaseBrain brain;

        private NeuronMenuButton selected;
        private NeuronMenuButton inputButton;
        private NeuronMenuButton outputButton;
        private NeuronMenuButton hiddenButton;
        
        private int prevWindowWidth;

        public EditorScene(Scene prevScene) : base(prevScene, new ImageWrapper(EinsteinPhiConfig.Render.DEFAULT_BACKGROUND))
        {
            brain = new BaseBrain(); // TODO load the brain from a bibite file

            selected = null;
            inputButton = new NeuronMenuButton(
                generateInputNeurons(),
                EinsteinPhiConfig.PAD,
                EinsteinPhiConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            outputButton = new NeuronMenuButton(
                generateOutputNeurons(),
                EinsteinPhiConfig.PAD,
                2 * EinsteinPhiConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs,
                onDeselectOutputs);
            hiddenButton = new NeuronMenuButton(
                generateHiddenNeurons(),
                EinsteinPhiConfig.PAD,
                3 * EinsteinPhiConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd,
                onDeselectAdd);
            prevWindowWidth = EinsteinPhiConfig.Window.WIDTH;
        }

        // ----------------------------------------------------------------
        //  Initialize and Close
        // ----------------------------------------------------------------

        protected override void InitializeMe()
        {
            inputButton.Initialize();
            foreach (NeuronDrawable neuronOption in inputButton.GetNeuronOptions())
            {
                IO.MOUSE.UP.SubscribeOnDrawable(() => {
                        onClickInputOption(neuronOption);
                    }, neuronOption);
            }
            outputButton.Initialize();
            foreach (NeuronDrawable neuronOption in outputButton.GetNeuronOptions())
            {
                IO.MOUSE.UP.SubscribeOnDrawable(() => {
                    onClickOutputOption(neuronOption);
                }, neuronOption);
            }
            hiddenButton.Initialize();
            // TODO set up onclicks
            IO.FRAME_TIMER.Subscribe(checkForResize);
        }

        protected override void CloseMe()
        {
            IO.FRAME_TIMER.Unsubscribe(checkForResize);
        }

        // ----------------------------------------------------------------
        //  Behavior
        // ----------------------------------------------------------------

        // TODO look at increasing performance of re-clicking the same drawable very quickly?

        // ----- OnClick of neuron options -----

        private void onClickInputOption(NeuronDrawable neuronOption)
        {
            inputButton.GetNeuronOptions().Remove(neuronOption);
            inputButton.RepositionOptions();
            neuronOption.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
            IO.MOUSE.UP.UnsubscribeAllFromDrawable(neuronOption);
            IO.MOUSE.CLICK.SubscribeOnDrawable(() => { onClickNeuron(neuronOption); }, neuronOption);
            Draggable drag = new Draggable(neuronOption, BRAIN_AREA);
            drag.Initialize();
        }

        private void onClickOutputOption(NeuronDrawable neuronOption)
        {
            outputButton.GetNeuronOptions().Remove(neuronOption);
            outputButton.RepositionOptions();
            neuronOption.SetCircleCenterXY(SPAWN_X, SPAWN_Y);
            IO.MOUSE.UP.UnsubscribeAllFromDrawable(neuronOption);
            IO.MOUSE.CLICK.SubscribeOnDrawable(() => { onClickNeuron(neuronOption); }, neuronOption);
            Draggable drag = new Draggable(neuronOption, BRAIN_AREA);
            drag.Initialize();
        }

        // ----- OnClick of neuron in brain -----

        private void onClickNeuron(NeuronDrawable neuron)
        {
            // TODO select this neuron and populate a menu of things you can do with it
            //      (e.g. hide/delete, ... maybe that's it?)
            // TODO make phi expose differentiation between left and right clicks
            Console.WriteLine("onclick");
        }

        // ----- Generating neuron options -----

        private static ICollection<NeuronDrawable> generateInputNeurons()
        {
            ICollection<NeuronDrawable> inputs = new List<NeuronDrawable>();
            for (int i = VersionConfig.INPUT_NODES_INDEX_MIN;
                i <= VersionConfig.INPUT_NODES_INDEX_MAX; i++)
            {
                inputs.Add(new NeuronDrawable(
                    i,
                    NeuronType.Input,
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return inputs;
        }

        private static ICollection<NeuronDrawable> generateOutputNeurons()
        {
            ICollection<NeuronDrawable> outputs = new List<NeuronDrawable>();
            for (int i = VersionConfig.OUTPUT_NODES_INDEX_MIN;
                i <= VersionConfig.OUTPUT_NODES_INDEX_MAX; i++)
            {
                outputs.Add(new NeuronDrawable(
                    i,
                    VersionConfig.GetOutputNeuronType(i),
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return outputs;
        }

        private static ICollection<NeuronDrawable> generateHiddenNeurons()
        {
            ICollection<NeuronDrawable> hiddens = new List<NeuronDrawable>();
            foreach (NeuronType neuronType in Enum.GetValues(typeof(NeuronType)))
            {
                if (neuronType == NeuronType.Input) { continue; }
                hiddens.Add(new NeuronDrawable(
                    VersionConfig.HIDDEN_NODES_INDEX_MAX,
                    neuronType));
            }
            return hiddens;
        }

        // ----- Button Selection -----

        private void onSelectInputs()
        {
            selected?.Deselect();
            selected = inputButton;
            selected.ShowOptions();
        }
        private void onSelectOutputs()
        {
            selected?.Deselect();
            selected = outputButton;
            selected.ShowOptions();
        }
        private void onSelectAdd()
        {
            selected?.Deselect();
            selected = hiddenButton;
            selected.ShowOptions();
        }

        private void onDeselectInputs() { selected?.HideOptions(); selected = null; }
        private void onDeselectOutputs() { selected?.HideOptions(); selected = null; }
        private void onDeselectAdd() { selected?.HideOptions(); selected = null; }

        public void repositionMenuButtons()
        {
            selected?.RepositionOptions();
        }


        // check if the window has been resized
        // if so, we probably need to reposition the menu buttons
        private void checkForResize()
        {
            if (IO.WINDOW.GetWidth() != prevWindowWidth)
            {
                inputButton.RepositionOptions();
                outputButton.RepositionOptions();
                hiddenButton.RepositionOptions();
            }
        }

    }
}
