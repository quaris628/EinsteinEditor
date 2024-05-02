using bibyte.functional.background;
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
    /// <summary>
    /// A number composed from a product of 2 input numbers
    /// </summary>
    public class ProductNum : Number
    {
        private Number left;
        private Number right;
        private JsonNeuron mult;

        public ProductNum(Number left, Number right)
        {
            this.left = left;
            this.right = right;
            this.mult = null;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (containsNonMults(outputConns) || mult != null)
            {
                // create a mult node in between the inputs and outputs
                if (mult == null)
                {
                    mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "Product");
                    left.ConnectTo(new[] { new ConnectToRequest(mult, 1f) });
                    right.ConnectTo(new[] { new ConnectToRequest(mult, 1f) });
                }
                connectAndHandleLargeScalars(mult, outputConns);
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
