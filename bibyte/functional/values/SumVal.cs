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
        private Value left;
        private Value right;

        public SumVal(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        public override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            if (containsMults(outputs))
            {
                // connect a linear node to the mult,
                // then connect values to that linear node
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Sum");
                left.AddOutput(linear);
                right.AddOutput(linear);
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(linear, output, 1);
                }
            }
            else
            {
                // connect values straight to the output node
                foreach (Neuron output in outputs)
                {
                    left.AddOutput(output);
                    right.AddOutput(output);
                }
            }
        }
    }
}
