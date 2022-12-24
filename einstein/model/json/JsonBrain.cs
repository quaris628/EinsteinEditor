using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class JsonBrain : BaseBrain
    {
        // Example:
        // {
        //  "isReady":true,
        //  "parent":true,
        //  "Nodes":[{Neuron},{Neuron},...{Neuron}],
        //  "Synapses":[{Synapse},{Synapse},...{Synapse}]
        // }

        private const string JSON_FORMAT =
            "{" +
            "\"isReady\":{0}," +
            "\"parent\":\"{1}\"," +
            "\"Nodes\":[{2}]," +
            "\"Synapses\":[{3}]" +
            "}";

        // unused for now, but they're in the json so keep track of them just in case
        private string isReady;
        private string parent;

        public JsonBrain() : base()
        {
            isReady = "true";
            parent = "true";
        }

        public JsonBrain(string json, int startIndex) : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            isReady = parser.getNextValue();
            parent = parser.getNextValue();

            // parse neurons
            parser.parseArray((neuronStartIndex) =>
            {
                BaseNeuron neuron = new JsonNeuron(json, neuronStartIndex);
                Add(neuron);
            });
            // parse synapses
            parser.parseArray((synapseStartIndex) =>
            {
                BaseSynapse synapse = new JsonSynapse(json, synapseStartIndex, this);
                Add(synapse);
            });
        }

        public override string GetSave()
        {
            return string.Format(JSON_FORMAT,
                isReady,
                parent,
                neuronsToJson(),
                synapsesToJson());
        }

        // these ToJson functions could probably be refactored to be less duplicately
        // but I'm too lazy right now and this code smell isn't very strong anyway
        private string neuronsToJson()
        {
            string[] neuronJsons = new string[Neurons.Count];
            int i = 0;
            foreach (JsonNeuron neuron in Neurons)
            {
                neuronJsons[i++] = neuron.GetSave();
            }
            return string.Join(",", neuronJsons);
        }

        private string synapsesToJson()
        {
            string[] neuronJsons = new string[Synapses.Count];
            int i = 0;
            foreach (JsonSynapse synapse in Synapses)
            {
                neuronJsons[i++] = synapse.GetSave();
            }
            return string.Join(",", neuronJsons);
        }
    }

}
