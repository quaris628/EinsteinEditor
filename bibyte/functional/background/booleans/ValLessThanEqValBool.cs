using bibyte.functional.background;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.booleans
{
    /// <summary>
    /// A boolean that is true if and only if the left number is less than or equal to the right number.
    /// 
    /// This is only an approximation of a less-than-or-equal-to.
    /// Once the left is less than or equal to the right, this will return true until
    /// the left is more than about 4x10^-6 i.e. 0.000004 greater than the right.
    /// </summary>
    public class ValLessThanEqValBool : Bool
    {
        public static float ERR_AFTER_LESS_THAN_OR_EQUAL = -0.000004f;

        private JsonNeuron latch;

        public ValLessThanEqValBool(Number left, Number right)
        {
            JsonNeuron sigmoid = NeuronFactory.CreateNeuron(NeuronType.Sigmoid, "ValLessThanEqVal");
            (left * -100f).ConnectTo(new[] { new ConnectToRequest(sigmoid, 1f) });
            (right * 100f).ConnectTo(new[] { new ConnectToRequest(sigmoid, 1f) });

            this.latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValLessThanEqVal");
            SynapseFactory.CreateSynapse(sigmoid, latch, 100);
            SynapseFactory.CreateSynapse(NeuronFactory.GetConst(), latch, -48.99999f);
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            foreach (ConnectToRequest outputConn in outputConns)
            {
                SynapseFactory.CreateSynapse(latch, outputConn.Neuron, outputConn.SynapseStrength);
            }
        }
    }
}
