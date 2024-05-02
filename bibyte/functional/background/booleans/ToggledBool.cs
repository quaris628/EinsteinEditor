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
    /// A boolean that is stored in memory, and its value is toggled when its input boolean is true.
    /// </summary>
    public class ToggledBool : Bool
    {
        private JsonNeuron memoryBit;

        public ToggledBool(Bool shouldToggle, bool initialState)
        {
            float initialStateFloat = initialState ? 1f : 0f;
            memoryBit = NeuronFactory.CreateNeuron(NeuronType.Latch, "toggledBoolMemoryBit",
                initialStateFloat, initialStateFloat, initialStateFloat);
            JsonNeuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "toggledBool",
                initialStateFloat, initialStateFloat, initialStateFloat);
            JsonNeuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "toggledBool",
                initialStateFloat, initialStateFloat, initialStateFloat);
            shouldToggle.ConnectTo(new[]
            {
                new ConnectToRequest(mult, 1f),
                new ConnectToRequest(linear, 1f),
            });
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, linear, 0.5f);
            SynapseFactory.CreateSynapse(memoryBit, mult, 1f);
            SynapseFactory.CreateSynapse(mult, linear, -2f);
            SynapseFactory.CreateSynapse(linear, memoryBit, 1f);
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            connectAndHandleLargeScalars(memoryBit, outputConns);
        }
    }
}
