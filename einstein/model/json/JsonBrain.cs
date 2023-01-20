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
            "    \"parent\": {1},\n" +
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
            isReady = "true";
            parent = "true";

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
            // I tried using .Union on these but this seriously looked like less work
            ICollection<BaseNeuron> allNeurons = EditorScene.generateInputNeurons();
            foreach (BaseNeuron neuron in EditorScene.generateOutputNeurons())
            {
                allNeurons.Add(neuron);
            }
            foreach (BaseNeuron neuron in Neurons)
            {
                bool alreadyInBrain = false;
                foreach (BaseNeuron neuron1 in allNeurons)
                {
                    if (neuron.Equals(neuron1))
                    {
                        alreadyInBrain = true;
                        break;
                    }
                }
                if (!alreadyInBrain)
                {
                    allNeurons.Add(neuron);
                }
            }

            string[] neuronJsons = new string[allNeurons.Count];
            int i = 0;
            foreach (JsonNeuron neuron in allNeurons)
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
