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
    /// A boolean that is true if and only if its input boolean changes from false to true.
    /// </summary>
    public class RisingBool : Bool
    {
        private Bool input;
        private JsonNeuron latch;

        public RisingBool(Bool input)
        {
            this.input = input;
            this.latch = null;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (input is ConstBool)
            {
                new ConstBool(false).ConnectTo(outputConns);
            }
            else
            {
                // if you change the number of synapses between the input and output,
                // then maybe update StoredValue accordingly
                // (TODO implement constant-timing stuff)

                if (latch == null)
                {
                    JsonNeuron diff = NeuronFactory.CreateNeuron(NeuronType.Differential, "RisingBool");
                    latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "RisingBool");
                    input.ConnectTo(new[] { new ConnectToRequest(diff, 100f) });
                    SynapseFactory.CreateSynapse(diff, latch, 100f);
                    SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -1f);
                }

                connectAndHandleLargeScalars(latch, outputConns);
            }
        }
    }
}
