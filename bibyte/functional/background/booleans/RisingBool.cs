using bibyte.functional.background;
using Bibyte.functional.background.values;
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
    /// 
    /// </summary>
    public class RisingBool : Bool
    {
        private Bool input;

        public RisingBool(Bool input)
        {
            this.input = input;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (input is ConstBool)
            {
                new ConstBool(false).ConnectTo(outputConns);
                return;
            }
            else
            {
                // if you change the number of synapses between the input and output,
                // then update StoredValue accordingly
                // (TODO implement constant-timing stuff)

                Neuron diff = NeuronFactory.CreateNeuron(NeuronType.Differential, "RisingBool");
                Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "RisingBool");
                input.ConnectTo(new [] {new ConnectToRequest(diff, 100f)});
                SynapseFactory.CreateSynapse(diff, latch, 100f);
                SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -1f);
                foreach (ConnectToRequest outputConn in outputConns)
                {
                    SynapseFactory.CreateSynapse(latch, outputConn.Neuron, outputConn.SynapseStrength);
                }
                
            }
        }
    }
}
