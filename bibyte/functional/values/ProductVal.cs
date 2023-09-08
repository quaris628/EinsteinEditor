using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    internal class ProductVal : Value
    {
        private List<Value> values = new List<Value>();
        public ProductVal(List<Value> values)
        {
            this.values = values;
        }
        public void AddVal(Value value)
        {
            values.Add(value);
        }
        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            IEnumerable<Synapse> synapses = new Synapse[] { };
            if (output.Type == NeuronType.Mult)
            {
                foreach (Value value in values)
                {
                    synapses = synapses.Concat(value.GetSynapsesTo(output));
                }
                return synapses.ToArray();
            }
            Neuron multNeuron = NeuronFactory.CreateNeuron(NeuronType.Mult);
            Synapse multToOutput = SynapseFactory.CreateSynapse(multNeuron, output, 1f);
            foreach (Value value in values)
            {
                synapses = synapses.Concat(value.GetSynapsesTo(multNeuron));
            }
            return synapses.ToArray();
        }
    }
}