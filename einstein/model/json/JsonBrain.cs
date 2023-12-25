using System;
using System.Collections.Generic;
using System.Globalization;
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
            "{\n" +
            "    \"isReady\": {0},\n" +
            "    \"parent\": {1},\n" +
            "    \"Nodes\": [\n" +
            "      {2}\n" +
            "    ],\n" +
            "    \"Synapses\": [\n" +
            "      {3}\n" +
            "    ]\n" +
            "  },\n";

        // unused for now, but they're in the json so keep track of them just in case
        private string isReady;
        private string parent;
        private Dictionary<int, int> oldNewNeuronIndicesMap;

        public JsonBrain() : base()
        {
            isReady = "true";
            parent = "true";
            oldNewNeuronIndicesMap = new Dictionary<int, int>();
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

            oldNewNeuronIndicesMap = new Dictionary<int, int>();
        }

        public override string GetSave()
        {
            return string.Format(CultureInfo.GetCultureInfo("en-US"),
                JSON_FORMAT,
                isReady,
                parent,
                neuronsToJson(),
                synapsesToJson());
        }

        // these ToJson functions could probably be refactored to be less duplicately
        // but I'm too lazy right now and this code smell isn't very strong anyway
        private string neuronsToJson()
        {
            // I tried using .Union on these but this seriously looked like less work
            List<BaseNeuron> allNeurons = new List<BaseNeuron>();
            foreach (BaseNeuron neuron in EditorScene.generateInputNeurons())
            {
                allNeurons.Add(neuron);
            }
            foreach (BaseNeuron neuron in EditorScene.generateOutputNeurons())
            {
                allNeurons.Add(neuron);
            }
            foreach (BaseNeuron newNeuron in Neurons)
            {
                bool alreadyInBrain = false;
                foreach (BaseNeuron oldNeuron in allNeurons)
                {
                    if (newNeuron.Equals(oldNeuron))
                    {
                        // Remove and replace b/c there may be hidden properties we
                        // want to preserve in the save, e.g. Inov
                        int indexOfOld = allNeurons.IndexOf(oldNeuron);
                        allNeurons.Remove(oldNeuron);
                        allNeurons.Insert(indexOfOld, newNeuron);
                        alreadyInBrain = true;
                        break;
                    }
                }
                if (!alreadyInBrain)
                {
                    allNeurons.Add(newNeuron);
                }
            }

            string[] neuronJsons = new string[allNeurons.Count];
            int i = 0;
            foreach (JsonNeuron neuron in allNeurons)
            {
                oldNewNeuronIndicesMap[neuron.Index] = i;
                JsonNeuron neuronCopy = new JsonNeuron(neuron);
                neuronCopy.YesImReallyAbsolutelyDefinitelySureIWantToChangeTheIndex(i);
                neuronJsons[i] = neuronCopy.GetSave();
                i++;
            }
            return string.Join(",\n      ", neuronJsons);
        }

        private string synapsesToJson()
        {
            string[] synapseJsons = new string[Synapses.Count];
            int i = 0;
            foreach (JsonSynapse synapse in Synapses)
            {
                int toIndex = oldNewNeuronIndicesMap[synapse.To.Index];
                int fromIndex = oldNewNeuronIndicesMap[synapse.From.Index];
                synapseJsons[i++] = synapse.GetSave(fromIndex, toIndex);
            }
            return string.Join(",\n      ", synapseJsons);
        }
    }

}
