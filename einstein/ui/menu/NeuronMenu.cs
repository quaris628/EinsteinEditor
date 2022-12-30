using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.menu
{
    public class NeuronMenu
    {
        private NeuronMenuButton selected;

        private NeuronMenuButton inputButton;
        private NeuronMenuButton outputButton;
        private NeuronMenuButton hiddenButton;

        public NeuronMenu()
        {
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
                onSelectOutputs, onDeselectOutputs);
            hiddenButton = new NeuronMenuButton(
                generateHiddenNeurons(),
                EinsteinPhiConfig.PAD,
                3 * EinsteinPhiConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd, onDeselectAdd);
        }

        public void Initialize()
        {
            inputButton.Initialize();
            outputButton.Initialize();
            hiddenButton.Initialize();
        }
        
        // TODO still need to set up onclicks of the individual neuron options

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

        // ----------------------------------------------------------------
        //  Generating lists of neurons
        // ----------------------------------------------------------------

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
    }
}
