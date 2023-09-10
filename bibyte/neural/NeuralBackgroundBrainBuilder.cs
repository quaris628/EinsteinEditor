using Bibyte.circuits;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;

namespace Bibyte.neural
{
    public static class NeuralBackgroundBrainBuilder
    {
        private static Brain brain = new Brain();

        public static void ClearBrain()
        {
            brain = new Brain();
        }

        public static void AddToBrain(Synapse[] synapses)
        {
            foreach (Synapse synapse in synapses)
            {
                AddToBrain(synapse);
            }
        }

        public static void AddToBrain(Synapse synapse)
        {
            AddSynapseAndItsNeurons(synapse);
        }

        private static void AddSynapseAndItsNeurons(Synapse synapse)
        {
            if (!brain.ContainsNeuron(synapse.From))
            {
                brain.Add(synapse.From);
            }
            if (!brain.ContainsNeuron(synapse.To))
            {
                brain.Add(synapse.To);
            }
            if (!brain.ContainsSynapse(synapse))
            {
                brain.Add(synapse);
            }
            else
            {
                Neuron lin = NeuronFactory.CreateNeuron(NeuronType.Linear, "DupeSyn");
                brain.Add(lin);
                brain.Add(new Synapse((Neuron)synapse.From, lin, synapse.Strength));
                brain.Add(new Synapse(lin, (Neuron)synapse.To, 1));
            }
        }

        public static Brain GetBrain() { return brain; }
    }
}
