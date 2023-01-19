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
        //   "isReady": true,
        //   "parent": true,
        //   "Nodes": [
        //     {
        //       Neuron
        //     },
        //     ...
        //     {
        //       Neuron
        //     }
        //   ],
        //   "Synapses": [
        //     {
        //       Synapse
        //     },
        //     ...
        //     {
        //       Synapse
        //     }
        //   ]
        // }

        private const string JSON_FORMAT =
            "    \"isReady\": {0},\n" +
            "    \"parent\": \"{1}\",\n" +
            "    \"Nodes\": [\n" +
            "      {2}\n" +
            "    ],\n" +
            "    \"Synapses\": [\n" +
            "      {3}\n" +
            "    ]\n";

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
            return "{\n" + string.Format(JSON_FORMAT,
                isReady,
                parent,
                neuronsToJson(),
                synapsesToJson()) + "  },\n";
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
            return string.Join(",\n      ", neuronJsons);
        }

        private string synapsesToJson()
        {
            string[] synapseJsons = new string[Synapses.Count];
            int i = 0;
            foreach (JsonSynapse synapse in Synapses)
            {
                synapseJsons[i++] = synapse.GetSave();
            }
            return string.Join(",\n      ", synapseJsons);
        }
    }

}
