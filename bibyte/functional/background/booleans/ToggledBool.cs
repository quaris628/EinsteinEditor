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
    public class ToggledBool : Bool
    {
        Bool shouldToggle;
        bool initalState;
        public ToggledBool(Bool shouldToggle, bool initialState)
        {
            this.shouldToggle = shouldToggle;
            this.initalState = initialState;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            Neuron memoryBit = NeuronFactory.CreateNeuron(NeuronType.Latch, "memoryBit");
            Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "memoryGate");
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
            shouldToggle.ConnectTo(new[]
            {
                new ConnectToRequest(mult, 1f),
                new ConnectToRequest(linear, 1f),
            });
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, linear, 0.5f);
            SynapseFactory.CreateSynapse(memoryBit, mult, 1f);
            SynapseFactory.CreateSynapse(mult, linear, -2f);
            SynapseFactory.CreateSynapse(linear, memoryBit, 1f);

            foreach (ConnectToRequest outputConn in outputConns)
            {
                SynapseFactory.CreateSynapse(memoryBit, outputConn.Neuron, outputConn.SynapseStrength);
            }
        }
    }
}
