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
                throw new ContainsDuplicateNeuronIndexException(neuron);
            }
            if (neuronDescriptionIndex.ContainsKey(neuron.Description))
            {
                throw new ContainsDuplicateNeuronDescriptionException(neuron);
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
                throw new ContainsDuplicateSynapseException(synapse);
            }

            Synapses.Add(synapse);
            // update indexes
            synapsesIndex.Add((synapse.From.Index, synapse.To.Index), synapse);
            synapsesFromIndex[synapse.From.Index].AddLast(synapse);
            synapsesToIndex[synapse.To.Index].AddLast(synapse);
        }

        public void Remove(BaseNeuron neuron)
        {
            if (!neuronsIndex.TryGetValue(neuron.Index, out BaseNeuron neuronToRemove)
                || !Neurons.Remove(neuronToRemove))
            {
                throw new ElementNotFoundException(
                    "Cannot remove the neuron '" +
                    neuron.ToString() + "' because it does not exist in the brain.");
            }
            // remove linked synapses
            // avoid concurrent modification exception
            LinkedList<BaseSynapse> linkedSynapses = new LinkedList<BaseSynapse>();
            foreach (BaseSynapse synapse in synapsesFromIndex[neuron.Index])
            {
                linkedSynapses.AddFirst(synapse);
            }
            foreach (BaseSynapse synapse in synapsesToIndex[neuron.Index])
            {
                if (synapse.From != synapse.To)
                {
                    linkedSynapses.AddFirst(synapse);
                }
            }
            foreach (BaseSynapse synapse in linkedSynapses)
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
            if (!synapsesIndex.TryGetValue((synapse.From.Index, synapse.To.Index),
                out BaseSynapse synapseToRemove)
                || !Synapses.Remove(synapseToRemove))
            {
                throw new ElementNotFoundException(
                    "Cannot remove the synapse '" + synapse +
                    "' because it does not exist in the brain.");
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
        public bool ContainsNeuronDescription(string description)
        {
            return neuronDescriptionIndex.ContainsKey(description);
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

        public void UpdateNeuronDescription(BaseNeuron neuron, string newDescription)
        {
            neuronDescriptionIndex.Remove(neuron.Description);
            neuronDescriptionIndex.Add(newDescription, neuron);
            neuron.Description = newDescription;
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

    public abstract class BrainException : Exception
    {
        private string title;
        protected BrainException(string title) : base() { this.title = title; }
        protected BrainException(string title, string message) : base(message) { this.title = title; }
        protected BrainException(string title, string message, Exception innerException)
            : base(message, innerException) { this.title = title; }
        public string GetDisplayMessage(string delimiter)
        {
            return title + delimiter + Message;
        }
    }

    public class ElementNotFoundException : BrainException
    {
        public const string TITLE = "Element not found";
        public ElementNotFoundException() : base(TITLE) { }
        public ElementNotFoundException(string message) : base(TITLE, message) { }
        public ElementNotFoundException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }

    public abstract class ContainsDuplicateException : BrainException
    {
        public const string TITLE = "Contains duplicate";
        protected ContainsDuplicateException() : base(TITLE) { }
        protected ContainsDuplicateException(string message) : base(TITLE, message) { }
        protected ContainsDuplicateException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }

    public class ContainsDuplicateNeuronIndexException : ContainsDuplicateException
    {
        public BaseNeuron Neuron { get; private set; }
        public ContainsDuplicateNeuronIndexException(BaseNeuron neuron)
            : base("This brain already has a neuron with index " + neuron.Index)
        {
            Neuron = neuron;
        }
    }
    public class ContainsDuplicateNeuronDescriptionException : ContainsDuplicateException
    {
        public BaseNeuron Neuron { get; private set; }
        public ContainsDuplicateNeuronDescriptionException(BaseNeuron neuron)
            : base("This brain already has a neuron with description '" + neuron.Description + "'")
        {
            Neuron = neuron;
        }
    }

    public class ContainsDuplicateSynapseException : ContainsDuplicateException
    {
        public BaseSynapse Synapse { get; private set; }
        public ContainsDuplicateSynapseException(BaseSynapse synapse)
            : base("This brain already has a synapse that connects {" +
                    synapse.From + "} to {" + synapse.To + "}")
        {
            Synapse = synapse;
        }
    }

}
