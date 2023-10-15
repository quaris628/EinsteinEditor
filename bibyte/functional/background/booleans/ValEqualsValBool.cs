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
    /// This is only an approximation of a less-than.
    /// Once the left is less than the right, this will return true until
    /// the left is more than about 1x10^-5 i.e. 0.00001 greater than the right.
    /// </summary>
    public class ValEqualsValBool : Bool
    {
        public static float ERR_AFTER_1 = 0.00001f;
        private Neuron latch;

        public ValEqualsValBool(Value left, Value right)
        {
            Neuron guassian = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "ValEqualsVal");
            (left * 100f).ConnectTo(new[] { guassian });
            (right * -100f).ConnectTo(new[] { guassian });

            this.latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValEqualsVal");
            SynapseFactory.CreateSynapse(guassian, latch, 100);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -98.99999f);
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
