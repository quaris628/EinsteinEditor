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

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            if (output.Type != NeuronType.Mult)
            {
                // connect values straight to the output node
                IEnumerable<Synapse> synapses = new Synapse[] {};
                foreach (Value val in values)
                {
                    synapses = synapses.Concat(val.GetSynapsesTo(output));
                }
                return synapses.ToArray();
            }
            else
            {
                // connect a linear node to the mult, then connect values to that linear node
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
                Synapse linToMult = SynapseFactory.CreateSynapse(linear, output, 1);
                IEnumerable<Synapse> synapses = new Synapse[] { linToMult };
                foreach (Value val in values)
                {
                    synapses = synapses.Concat(val.GetSynapsesTo(linear));
                }
                return synapses.ToArray();
            }
        }
    }
}
