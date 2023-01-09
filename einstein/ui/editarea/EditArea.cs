using Einstein.model;
using Einstein.ui.menu;
using phi.graphics.renderables;
using phi.io;
using phi.other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class EditArea
    {
        public static readonly Rectangle BOUNDS = new Rectangle(
            NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3,
            EinsteinPhiConfig.Window.WIDTH - NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD,
            EinsteinPhiConfig.Window.HEIGHT - (NeuronMenuButton.HEIGHT + EinsteinPhiConfig.PAD) * 3);

        private BaseBrain brain;

        private Dictionary<int, NeuronRenderable> displayedNeuronsIndex;
        private Action<BaseNeuron> onRemove;
        private Dictionary<(int, int), SynapseRenderable> displayedSynapsesIndex;
        private SynapseRenderable startedSynapse;
        private int hiddenNeuronIndex;

        public EditArea(BaseBrain brain, Action<BaseNeuron> onRemove)
        {
            this.brain = brain;
            displayedNeuronsIndex = new Dictionary<int, NeuronRenderable>();
            this.onRemove = onRemove;
            displayedSynapsesIndex = new Dictionary<(int, int), SynapseRenderable>();
            hiddenNeuronIndex = VersionConfig.HIDDEN_NODES_INDEX_MIN;
        }

        public void AddNeuron(BaseNeuron neuron)
        {
            brain.Add(neuron);

            NeuronRenderable dragNeuron = new NeuronRenderable(this, neuron);
            dragNeuron.Initialize();
            displayedNeuronsIndex.Add(neuron.Index, dragNeuron);
        }

        public void CreateHiddenNeuron(NeuronType type)
        {
            AddNeuron(new BaseNeuron(hiddenNeuronIndex++, type));
        }

        public void RemoveNeuron(BaseNeuron neuron)
        {
            // remove connecting synapses first
            // can't remove inside the loops b/c would be concurrent modification
            LinkedList<BaseSynapse> toRemove = new LinkedList<BaseSynapse>();
            foreach (BaseSynapse from in brain.GetSynapsesFrom(neuron))
            {
                toRemove.AddFirst(from);
            }
            foreach (BaseSynapse to in brain.GetSynapsesTo(neuron))
            {
                toRemove.AddFirst(to);
            }
            foreach (BaseSynapse synapse in toRemove)
            {
                RemoveSynapse(synapse);
            }

            brain.Remove(neuron);

            NeuronRenderable dragNeuron = displayedNeuronsIndex[neuron.Index];
            dragNeuron.Uninitialize();
            displayedNeuronsIndex.Remove(neuron.Index);

            onRemove.Invoke(neuron);
        }

        public void StartSynapse(NeuronRenderable from, int x, int y)
        {
            startedSynapse = new SynapseRenderable(this, from, x, y);
            startedSynapse.Initialize();
        }

        public void FinishSynapse(BaseSynapse synapse)
        {
            brain.Add(synapse);

            displayedSynapsesIndex.Add((synapse.From.Index, synapse.To.Index), startedSynapse);
        }

        public void RemoveSynapse(BaseSynapse synapse)
        {
            brain.Remove(synapse);

            (int, int) key = (synapse.From.Index, synapse.To.Index);
            displayedSynapsesIndex[key].Uninitialize();
            displayedSynapsesIndex.Remove(key);
        }

        // if there are none, returns null
        public NeuronRenderable HasNeuronAtPosition(int x, int y)
        {
            foreach (NeuronRenderable dragNeuron in displayedNeuronsIndex.Values)
            {
                if (dragNeuron.GetDrawable().GetBoundaryRectangle().Contains(x, y)
                    && dragNeuron.GetDrawable().IsDisplaying())
                {
                    return dragNeuron;
                }
            }
            return null;
        }
    }
}
