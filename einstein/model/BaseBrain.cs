using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseBrain
    {
        public ICollection<BaseNeuron> Neurons { get; }
        private Dictionary<int, BaseNeuron> neuronsIndex;
        private Dictionary<string, BaseNeuron> neuronDescriptionIndex;

        public ICollection<BaseSynapse> Synapses { get; }
        private Dictionary<(int, int), BaseSynapse> synapsesIndex;
        private Dictionary<int, LinkedList<BaseSynapse>> synapsesFromIndex;
        private Dictionary<int, LinkedList<BaseSynapse>> synapsesToIndex;

        public BaseBrain()
        {
            Neurons = new LinkedList<BaseNeuron>();
            Synapses = new LinkedList<BaseSynapse>();
            neuronsIndex = new Dictionary<int, BaseNeuron>();
            neuronDescriptionIndex = new Dictionary<string, BaseNeuron>();
            synapsesIndex = new Dictionary<(int, int), BaseSynapse>();
            synapsesFromIndex = new Dictionary<int, LinkedList<BaseSynapse>>();
            synapsesToIndex = new Dictionary<int, LinkedList<BaseSynapse>>();
        }

        public void Add(BaseNeuron neuron)
        {
            if (neuronsIndex.ContainsKey(neuron.Index))
            {
                throw new ContainsDuplicateException(
                    "This brain already has a neuron with index "
                    + neuron.Index);
            }
            if (neuronDescriptionIndex.ContainsKey(neuron.Description))
            {
                throw new ContainsDuplicateException(
                    "This brain already has a neuron with description '" +
                    neuron.Description + "'");
            }

            Neurons.Add(neuron);
            // update indexes
            neuronsIndex.Add(neuron.Index, neuron);
            neuronDescriptionIndex.Add(neuron.Description, neuron);
            synapsesFromIndex[neuron.Index] = new LinkedList<BaseSynapse>();
            synapsesToIndex[neuron.Index] = new LinkedList<BaseSynapse>();
        }

        public void Add(BaseSynapse synapse)
        {
            if (!neuronsIndex.ContainsKey(synapse.From.Index))
            {
                throw new ElementNotFoundException(
                    "This brain does not contain the neuron '" +
                    synapse.From.ToString() + "'");
            }
            if (!neuronsIndex.ContainsKey(synapse.To.Index))
            {
                throw new ElementNotFoundException(
                    "This brain does not contain the neuron '" +
                    synapse.To.ToString() + "'");
            }
            if (synapsesIndex.ContainsKey((synapse.From.Index, synapse.To.Index)))
            {
                throw new ContainsDuplicateException(
                    "This brain already has a synapse that connects '" +
                    synapse.From + "' to '" + synapse.To + "'");
            }

            Synapses.Add(synapse);
            // update indexes
            synapsesIndex.Add((synapse.From.Index, synapse.To.Index), synapse);
            synapsesFromIndex[synapse.From.Index].AddLast(synapse);
            synapsesToIndex[synapse.To.Index].AddLast(synapse);
        }

        public void Remove(BaseNeuron neuron)
        {
            if (!Neurons.Remove(neuron))
            {
                throw new ElementNotFoundException(
                    "This brain does not have the neuron '" +
                    neuron.ToString() + "'");
            }
            // remove linked synapses
            foreach (BaseSynapse synapse in synapsesFromIndex[neuron.Index])
            {
                Remove(synapse);
            }
            foreach (BaseSynapse synapse in synapsesToIndex[neuron.Index])
            {
                Remove(synapse);
            }
            // update indexes
            neuronsIndex.Remove(neuron.Index);
            neuronDescriptionIndex.Remove(neuron.Description);
            synapsesFromIndex.Remove(neuron.Index);
            synapsesToIndex.Remove(neuron.Index);
        }

        public void Remove(BaseSynapse synapse)
        {
            if (!Synapses.Remove(synapse))
            {
                throw new ElementNotFoundException(
                    "This brain does not have the synapse '" +
                    synapse.ToString() + "'");
            }
            // update indexes
            synapsesIndex.Remove((synapse.From.Index, synapse.To.Index));
            synapsesFromIndex[synapse.From.Index].Remove(synapse);
            synapsesToIndex[synapse.To.Index].Remove(synapse);
        }

        public bool ContainsNeuron(BaseNeuron neuron) { return ContainsNeuron(neuron.Index); }
        public bool ContainsNeuron(int index)
        {
            return neuronsIndex.ContainsKey(index);
        }

        public bool ContainsSynapse(BaseSynapse synapse) { return ContainsSynapse(synapse.From.Index, synapse.To.Index); }
        public bool ContainsSynapse(int fromIndex, int toIndex)
        {
            return synapsesIndex.ContainsKey((fromIndex, toIndex));
        }

        public BaseNeuron GetNeuron(int index)
        {
            return neuronsIndex[index];
        }

        public BaseNeuron GetNeuron(string description)
        {
            return neuronDescriptionIndex[description];
        }

        public BaseSynapse GetSynapse(BaseNeuron from, BaseNeuron to) { return GetSynapse(from.Index, to.Index); }
        public BaseSynapse GetSynapse(int fromIndex, int toIndex)
        {
            return synapsesIndex[(fromIndex, toIndex)];
        }

        public ICollection<BaseSynapse> GetSynapsesFrom(BaseNeuron neuron) { return GetSynapsesFrom(neuron.Index); }
        public ICollection<BaseSynapse> GetSynapsesFrom(int neuronIndex)
        {
            return synapsesFromIndex[neuronIndex];
        }

        public ICollection<BaseSynapse> GetSynapsesTo(BaseNeuron neuron) { return GetSynapsesTo(neuron.Index); }
        public ICollection<BaseSynapse> GetSynapsesTo(int neuronIndex)
        {
            return synapsesToIndex[neuronIndex];
        }

        public virtual string GetSave() { throw new NotSupportedException(); }
    }

    public class ContainsDuplicateException : Exception {
        public ContainsDuplicateException() : base() { }
        public ContainsDuplicateException(string message) : base(message) { }
        public ContainsDuplicateException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException() : base() { }
        public ElementNotFoundException(string message) : base(message) { }
        public ElementNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }

}
