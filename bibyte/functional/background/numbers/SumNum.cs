using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background
{
    /// <summary>
    /// A number composed from a sum of 2 input numbers
    /// </summary>
    public class SumNum : Number
    {
        private Number left;
        private Number right;
        private Neuron linear;

        public SumNum(Number left, Number right)
        {
            this.left = left;
            this.right = right;
            this.linear = null;
        }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            if (containsMults(outputs) || linear != null)
            {
                // create a linear node in between the inputs and outputs
                if (linear == null)
                {
                    linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Sum");
                    left.ConnectTo(new[] { linear });
                    right.ConnectTo(new[] { linear });
                }
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
                    left.ConnectTo(new[] { output });
                    right.ConnectTo(new[] { output });
                }
            }
        }
    }
}
