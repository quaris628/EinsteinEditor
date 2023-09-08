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
        public override void AddSynapsesTo(Neuron output)
        {
            if (output.Type == NeuronType.Mult)
            {
                foreach (Value value in values)
                {
                    value.AddSynapsesTo(output);
                }
            }
            else
            {
                Neuron multNeuron = NeuronFactory.CreateNeuron(NeuronType.Mult);
                foreach (Value value in values)
                {
                    value.AddSynapsesTo(multNeuron);
                }
                SynapseFactory.CreateSynapse(multNeuron, output, 1f);
            }
        }
    }
}