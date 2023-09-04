using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class Brain : BaseBrain
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
        private Dictionary<int, int> oldNewNeuronIndicesMap;

        public Brain() : base()
        {
            isReady = "true";
            parent = "true";
            oldNewNeuronIndicesMap = new Dictionary<int, int>();
        }

        public Brain(string json, int startIndex) : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            isReady = "true";
            parent = "true";

            // parse neurons
            parser.parseArray((neuronStartIndex) =>
            {
                BaseNeuron neuron = new Neuron(json, neuronStartIndex);
                Add(neuron);
            });
            // parse synapses
            parser.parseArray((synapseStartIndex) =>
            {
                BaseSynapse synapse = new Synapse(json, synapseStartIndex, this);
                Add(synapse);
            });

            oldNewNeuronIndicesMap = new Dictionary<int, int>();
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
            foreach (Neuron neuron in allNeurons)
            {
                oldNewNeuronIndicesMap[neuron.Index] = i;
                Neuron neuronCopy = new Neuron(neuron);
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
            foreach (Synapse synapse in Synapses)
            {
                int toIndex = oldNewNeuronIndicesMap[synapse.To.Index];
                int fromIndex = oldNewNeuronIndicesMap[synapse.From.Index];
                synapseJsons[i++] = synapse.GetSave(fromIndex, toIndex);
            }
            return string.Join(",\n      ", synapseJsons);
        }
    }

}
