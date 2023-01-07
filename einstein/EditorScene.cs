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
using Einstein.ui.editarea;

namespace Einstein
{
    class EditorScene : Scene
    {
        // ----------------------------------------------------------------
        //  Config
        // ----------------------------------------------------------------


        // ----------------------------------------------------------------
        //  Data/Constructor
        // ----------------------------------------------------------------

        private EditArea editArea;

        // TODO maybe refactor the input/output/hidden buttons into one renderable class
        private NeuronMenuCategory selected;
        private IONeuronMenuCategory input;
        private IONeuronMenuCategory output;
        private NeuronMenuCategory hidden;

        private int prevWindowWidth;

        public EditorScene(Scene prevScene) : base(prevScene, new ImageWrapper(EinsteinPhiConfig.Render.DEFAULT_BACKGROUND))
        {
            editArea = new EditArea(
                new BaseBrain(), // TODO load the brain from a bibite file
                (neuronRemovedFromEditArea) => {
                    if (neuronRemovedFromEditArea.IsInput())
                    {
                        input.AddOption(neuronRemovedFromEditArea);
                    }
                    else if (neuronRemovedFromEditArea.IsOutput())
                    {
                        output.AddOption(neuronRemovedFromEditArea);
                    }
                });

            selected = null;
            NeuronMenuButton inputButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                EinsteinPhiConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            input = new IONeuronMenuCategory(
                inputButton,
                generateInputNeurons(),
                (neuronRemovedFromMenu) => {
                    editArea.AddNeuron(neuronRemovedFromMenu);
                });

            NeuronMenuButton outputButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                2 * EinsteinPhiConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs,
                onDeselectOutputs);
            output = new IONeuronMenuCategory(
                outputButton,
                generateOutputNeurons(),
                (neuronRemovedFromMenu) => {
                    editArea.AddNeuron(neuronRemovedFromMenu);
                });

            NeuronMenuButton hiddenButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                3 * EinsteinPhiConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd,
                onDeselectAdd);
            hidden = new NeuronMenuCategory(
                hiddenButton,
                generateHiddenNeurons());
            // TODO make the hidden neuron menu do stuff

            prevWindowWidth = EinsteinPhiConfig.Window.WIDTH;
        }

        // ----------------------------------------------------------------
        //  Initialize and Close
        // ----------------------------------------------------------------

        protected override void InitializeMe()
        {
            input.Initialize();
            output.Initialize();
            hidden.Initialize();
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

        // ----- Generating neuron options -----

        private static ICollection<BaseNeuron> generateInputNeurons()
        {
            ICollection<BaseNeuron> inputs = new List<BaseNeuron>();
            for (int i = VersionConfig.INPUT_NODES_INDEX_MIN;
                i <= VersionConfig.INPUT_NODES_INDEX_MAX; i++)
            {
                inputs.Add(new BaseNeuron(
                    i,
                    NeuronType.Input,
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return inputs;
        }

        private static ICollection<BaseNeuron> generateOutputNeurons()
        {
            ICollection<BaseNeuron> outputs = new List<BaseNeuron>();
            for (int i = VersionConfig.OUTPUT_NODES_INDEX_MIN;
                i <= VersionConfig.OUTPUT_NODES_INDEX_MAX; i++)
            {
                outputs.Add(new BaseNeuron(
                    i,
                    VersionConfig.GetOutputNeuronType(i),
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return outputs;
        }

        private static ICollection<BaseNeuron> generateHiddenNeurons()
        {
            ICollection<BaseNeuron> hiddens = new List<BaseNeuron>();
            int index = VersionConfig.HIDDEN_NODES_INDEX_MAX
                - Enum.GetValues(typeof(NeuronType)).Length;
            foreach (NeuronType neuronType in Enum.GetValues(typeof(NeuronType)))
            {
                if (neuronType == NeuronType.Input) { continue; }
                hiddens.Add(new BaseNeuron(
                    index++,
                    neuronType));
            }
            return hiddens;
        }

        // ----- Button Selection -----

        private void onSelectInputs()
        {
            selected?.Button.Deselect();
            selected = input;
        }
        private void onSelectOutputs()
        {
            selected?.Button.Deselect();
            selected = output;
        }
        private void onSelectAdd()
        {
            selected?.Button.Deselect();
            selected = hidden;
        }

        private void onDeselectInputs() { selected = null; }
        private void onDeselectOutputs() { selected = null; }
        private void onDeselectAdd() { selected = null; }

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
                input.RepositionOptions();
                output.RepositionOptions();
                hidden.RepositionOptions();
            }
        }

    }
}
