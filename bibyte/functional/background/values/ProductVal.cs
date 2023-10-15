using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    internal class ProductVal : Value
    {
        private Value left;
        private Value right;
        public ProductVal(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            if (containsNonMults(outputs))
            {
                // connect a mult node to the neuron,
                // then connect values to that mult node
                Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "Product");
                left.ConnectTo(new[] { mult });
                right.ConnectTo(new[] { mult });
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(mult, output, 1);
                }
            }
            else
            {
                // connect values straight to the output node
                foreach (Neuron output in outputs)
                {
                    left.ConnectTo(new[] { output });
                    right.ConnectTo(new[] { output });
                }
            }
        }
    }
}
