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
            ICollection<NeuronDrawable> inputs = new LinkedList<NeuronDrawable>();
            ICollection<NeuronDrawable> outputs = new LinkedList<NeuronDrawable>();
            ICollection<NeuronDrawable> hiddenTypes = new LinkedList<NeuronDrawable>();
            // TODO populate ^
            // this is for now
            inputs.Add(new NeuronDrawable(0, NeuronType.Input, "Constant"));
            outputs.Add(new NeuronDrawable(1, NeuronType.TanH, "Accelerate"));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Sigmoid, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Linear, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.TanH, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Sine, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.ReLu, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Gaussian, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Latch, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Differential, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Abs, ""));
            hiddenTypes.Add(new NeuronDrawable(VersionConfig.HIDDEN_NODES_INDEX_MAX, NeuronType.Mult, ""));

            selected = null;
            inputButton = new NeuronMenuButton(
                inputs,
                EinsteinPhiConfig.PAD,
                EinsteinPhiConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            outputButton = new NeuronMenuButton(
                outputs,
                EinsteinPhiConfig.PAD,
                2 * EinsteinPhiConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs, onDeselectOutputs);
            hiddenButton = new NeuronMenuButton(
                hiddenTypes,
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
    }
}
