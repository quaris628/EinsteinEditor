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
    public class SumVal : Value
    {
        private List<Value> values;

        public SumVal()
        {
            values = new List<Value>();
        }

        public void Add(Value val)
        {
            values.Add(val);
        }

        public override void AddSynapsesTo(Neuron output)
        {
            if (output.Type != NeuronType.Mult)
            {
                // connect values straight to the output node
                foreach (Value val in values)
                {
                    val.AddSynapsesTo(output);
                }
            }
            else
            {
                // connect a linear node to the mult, then connect values to that linear node
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Sum");
                foreach (Value val in values)
                {
                    val.AddSynapsesTo(linear);
                }
                SynapseFactory.CreateSynapse(linear, output, 1);
            }
        }
    }
}
