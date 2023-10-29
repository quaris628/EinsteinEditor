using bibyte.functional.background;
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

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (containsMults(outputConns) || linear != null)
            {
                // create a linear node in between the inputs and outputs
                if (linear == null)
                {
                    linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Sum");
                    left.ConnectTo(new[] { new ConnectToRequest(linear, 1f) });
                    right.ConnectTo(new[] { new ConnectToRequest(linear, 1f) });
                }
                connectAndHandleLargeScalars(linear, outputConns);
            }
            else
            {
                // connect values straight to the output node
                left.ConnectTo(outputConns);
                right.ConnectTo(outputConns);
            }
        }
    }
}
