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
    /// A boolean that is true if and only if the left number is equal to the right number.
    /// 
    /// This is only an approximation of an equals.
    /// TODO describe the subtleties
    /// </summary>
    public class ValEqualsValBool : Bool
    {
        public static float ERR_AFTER_1 = 0.00001f;
        private JsonNeuron latch;

        public ValEqualsValBool(Number left, Number right)
        {
            JsonNeuron guassian = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "ValEqualsVal");
            (left * 100f).ConnectTo(new[] { new ConnectToRequest(guassian, 1f) });
            (right * -100f).ConnectTo(new[] { new ConnectToRequest(guassian, 1f) });

            latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValEqualsVal");
            SynapseFactory.CreateSynapse(guassian, latch, 100);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -98.99999f);
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            connectAndHandleLargeScalars(latch, outputConns);
        }
    }
}
