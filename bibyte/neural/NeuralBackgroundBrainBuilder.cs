using Bibyte.circuits;
using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;

namespace Bibyte.neural
{
    public static class NeuralBackgroundBrainBuilder
    {
        private static JsonBrain brain = null;

        public static void ClearBrain(BibiteVersion bibiteVersion)
        {
            brain = new JsonBrain(bibiteVersion);
        }

        public static void AddToBrain(JsonSynapse[] synapses)
        {
            foreach (JsonSynapse synapse in synapses)
            {
                AddToBrain(synapse);
            }
        }

        public static void AddToBrain(JsonSynapse synapse)
        {
            AddSynapseAndItsNeurons(synapse);
        }

        private static void AddSynapseAndItsNeurons(JsonSynapse synapse)
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
                JsonNeuron lin = NeuronFactory.CreateNeuron(NeuronType.Linear, "DupeSyn");
                brain.Add(lin);
                brain.Add(new JsonSynapse((JsonNeuron)synapse.From, lin, synapse.Strength));
                brain.Add(new JsonSynapse(lin, (JsonNeuron)synapse.To, 1));
            }
        }

        public static JsonBrain GetBrain() { return brain; }
    }
}
