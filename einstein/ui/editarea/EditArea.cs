using Einstein.model;
using Einstein.model.json;
using Einstein.ui.menu;
using phi.graphics.renderables;
using phi.io;
using phi.other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class EditArea
    {
        public BaseBrain Brain { get; private set; }

        private Dictionary<int, NeuronRenderable> displayedNeuronsIndex;
        private Action<BaseNeuron> onRemove;
        private bool disableOnRemove;
        private Dictionary<(int, int), SynapseRenderable> displayedSynapsesIndex;
        private SynapseRenderable startedSynapse;
        private int hiddenNeuronIndex;

        public EditArea(BaseBrain brain, Action<BaseNeuron> onRemove)
        {
            displayedNeuronsIndex = new Dictionary<int, NeuronRenderable>();
            this.onRemove = onRemove;
            disableOnRemove = false;
            displayedSynapsesIndex = new Dictionary<(int, int), SynapseRenderable>();
            hiddenNeuronIndex = BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN;
            LoadBrain(brain);
        }

        public void AddNeuron(BaseNeuron neuron)
        {
            Brain?.Add(neuron);

            NeuronRenderable dragNeuron = new NeuronRenderable(this, neuron);
            dragNeuron.Initialize();
            displayedNeuronsIndex.Add(neuron.Index, dragNeuron);
        }

        public void CreateHiddenNeuron(NeuronType type)
        {
            AddNeuron(new JsonNeuron(hiddenNeuronIndex++, type,
                type.ToString() + (hiddenNeuronIndex - BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN)));
        }

        public void RemoveNeuron(BaseNeuron neuron)
        {
            // remove connecting synapses first
            // can't remove inside the loops b/c would be concurrent modification
            LinkedList<BaseSynapse> toRemove = new LinkedList<BaseSynapse>();
            foreach (BaseSynapse from in Brain.GetSynapsesFrom(neuron))
            {
                toRemove.AddFirst(from);
            }
            foreach (BaseSynapse to in Brain.GetSynapsesTo(neuron))
            {
                toRemove.AddFirst(to);
            }
            foreach (BaseSynapse synapse in toRemove)
            {
                RemoveSynapse(synapse);
            }

            Brain.Remove(neuron);

            NeuronRenderable dragNeuron = displayedNeuronsIndex[neuron.Index];
            dragNeuron.Uninitialize();
            displayedNeuronsIndex.Remove(neuron.Index);

            if (!disableOnRemove)
            {
                onRemove.Invoke(neuron);
            }
        }

        public void StartSynapse(NeuronRenderable from, int x, int y)
        {
            startedSynapse = new SynapseRenderable(this, from, x, y);
            startedSynapse.Initialize();
        }

        // be careful using, should probably only be run from Finalize inside SynapseRenderable
        public void FinishSynapse(BaseSynapse synapse)
        {
            if (startedSynapse == null)
            {
                throw new InvalidOperationException(
                    "Every FinishSynapse call must be paired with a prior StartSynapse call");
            }
            Brain?.Add(synapse);

            displayedSynapsesIndex.Add((synapse.From.Index, synapse.To.Index), startedSynapse);
            startedSynapse = null;
        }

        // This is a much safer option for anyone who doesn't know what they're doing
        // (including you Quaris, if you're unable to remember or focus)
        public void AddSynapse(BaseSynapse synapse)
        {
            StartSynapse(displayedNeuronsIndex[synapse.From.Index],
                NeuronRenderable.SPAWN_X, NeuronRenderable.SPAWN_Y);
            SynapseRenderable synapseR = startedSynapse;
            startedSynapse.Finalize(displayedNeuronsIndex[synapse.To.Index], synapse);
        }

        public void RemoveSynapse(BaseSynapse synapse)
        {
            Brain.Remove(synapse);

            (int, int) key = (synapse.From.Index, synapse.To.Index);
            displayedSynapsesIndex[key].Uninitialize();
            displayedSynapsesIndex.Remove(key);
        }

        public void LoadBrain(BaseBrain brain)
        {
            // CLear any data currently in the brain
            NeuronRenderable[] neuronsToClearFromOldBrain = new NeuronRenderable[displayedNeuronsIndex.Values.Count];
            displayedNeuronsIndex.Values.CopyTo(neuronsToClearFromOldBrain, 0);
            disableOnRemove = true;
            foreach (NeuronRenderable nr in neuronsToClearFromOldBrain)
            {
                RemoveNeuron(nr.Neuron);
            }
            disableOnRemove = false;
            displayedNeuronsIndex = new Dictionary<int, NeuronRenderable>();
            displayedSynapsesIndex = new Dictionary<(int, int), SynapseRenderable>();
            hiddenNeuronIndex = BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN;
            Brain = null;

            // Add all neurons (except neurons that aren't connected to anything)
            LinkedList<BaseNeuron> neuronsToRemoveFromNewBrain = new LinkedList<BaseNeuron>(); // avoid concurrent modification
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                if (brain.GetSynapsesFrom(neuron).Count == 0
                    && brain.GetSynapsesTo(neuron).Count == 0)
                {
                    neuronsToRemoveFromNewBrain.AddFirst(neuron);
                    continue;
                }
                AddNeuron(neuron);
                if (neuron.Index >= hiddenNeuronIndex)
                {
                    hiddenNeuronIndex = neuron.Index + 1;
                }
            }
            foreach (BaseNeuron neuron in neuronsToRemoveFromNewBrain)
            {
                brain.Remove(neuron);
            }

            // Add all synapses
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                AddSynapse(synapse);
            }

            // Ensures shallow copies are equal (in case that matters) and more importantly,
            // if any Neuron/Synapse/Brain class is a subclass of the base class, that is preserved
            Brain = brain;
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
        public static Rectangle GetBounds()
        {
            return new Rectangle(
                NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD * 2,
                0,
                IO.WINDOW.GetWidth() - NeuronMenuButton.WIDTH + EinsteinPhiConfig.PAD * 2,
                IO.WINDOW.GetHeight());
        }

        public string LogDetailsForCrash()
        {
            string log = "";
            try
            {
                log += "\nBrain = " + Brain.GetSave();
            } catch (Exception) { }
            log += "\ndisplayedNeuronsIndex = ";
            if (displayedNeuronsIndex == null) { log += "null"; }
            else if (displayedNeuronsIndex.Count == 0) { log += "empty"; }
            else { log += string.Join(":", displayedNeuronsIndex); }
            log += "\nonRemove.Method.Name = " + onRemove.Method.Name;
            log += "\nonRemove.Method.GetParameters() = " + string.Join<ParameterInfo>(",", onRemove.Method.GetParameters());
            log += "\nonRemove.Method.ReturnType = " + onRemove.Method.ReturnType;
            log += "\nonRemove.Method.GetMethodBody().LocalVariables = ";
            if (onRemove.Method.GetMethodBody().LocalVariables == null) { log += "null"; }
            else if (onRemove.Method.GetMethodBody().LocalVariables.Count == 0) { log += "empty"; }
            else { log += string.Join(",", onRemove.Method.GetMethodBody().LocalVariables); }
            log += "\nonRemove.Method.Name = " + onRemove.Method.Name;
            log += "\nonRemove.Target = " + onRemove.Target;
            log += "\ndisableOnRemove = " + disableOnRemove;
            log += "\ndisplayedSynapsesIndex = ";
            if (displayedSynapsesIndex == null) { log += "null"; }
            else if (displayedSynapsesIndex.Count == 0) { log += "empty"; }
            else { log += string.Join(":", displayedSynapsesIndex); }
            log += "\nstartedSynapse = " + (startedSynapse != null ? startedSynapse.ToString() : "null");
            log += "\nhiddenNeuronIndex = " + hiddenNeuronIndex;
            return log;
        }
    }
}
