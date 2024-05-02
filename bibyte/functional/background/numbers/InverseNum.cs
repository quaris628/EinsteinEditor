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
    /// A number that is 1 / an input number
    /// 
    /// This is only an approximation of division and breaks when the denominator is near zero.
    /// The error is less than 1% when the denominator is farther than 0.1 from zero,
    /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
    /// </summary>
    public class InverseNum : Number
    {
        private Number val;
        private JsonNeuron gauss;
        private JsonNeuron mult;

        public InverseNum(Number val)
        {
            // could get more and more precise with more parallel nodes
            // (optional, could do later)

            this.val = val;
            gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "Inverse");
            val.ConnectTo(new[] { new ConnectToRequest(gauss, 100f) });
            mult = null;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (containsNonMults(outputConns) || mult != null)
            {
                if (mult == null)
                {
                    mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "Inverse");
                    val.ConnectTo(new[]
                    {
                        new ConnectToRequest(mult, 100f),
                    });
                    SynapseFactory.CreateSynapse(gauss, mult, 100f);
                }
                connectAndHandleLargeScalars(mult, outputConns);
            }
            else
            {
                IEnumerable<ConnectToRequest> outputConnsX100 = multiplyAllConnsBy(outputConns, 100f);
                connectAndHandleLargeScalars(gauss, outputConnsX100);
                val.ConnectTo(outputConnsX100);
            }
        }
    }
}
