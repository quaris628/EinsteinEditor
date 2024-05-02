using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bibyte.neural;
using Einstein.model;
using bibyte.functional.background;

namespace Bibyte.functional.background.booleans
{
    /// <summary>
    /// A boolean stored in memory.
    /// Takes 2 inputs:
    ///  - A boolean that, when true, will overwrite the stored boolean with the input boolean to store
    ///  - The boolean to be stored when the should-store boolean is true
    /// </summary>
    public class StoredBool : Bool
    {
        private JsonNeuron memoryBit;

        public StoredBool(Bool shouldStore, Bool valueToStoreFrom)
            : this(shouldStore, valueToStoreFrom, false) { }
        public StoredBool(Bool shouldStore, Bool valueToStoreFrom, bool initialValue)
        {
            float initialValueFloat = initialValue ? 1f : 0f;
            JsonNeuron memoryShouldStoreMult = NeuronFactory.CreateNeuron(NeuronType.Mult, "memoryShouldStoreMult");

            memoryBit = NeuronFactory.CreateNeuron(NeuronType.Latch, "memoryBit",
                initialValueFloat, initialValueFloat, initialValueFloat);
            SynapseFactory.CreateSynapse(memoryShouldStoreMult, memoryBit, 1f);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, memoryBit, 0.5f);
            shouldStore.ConnectTo(new[]
            {
                new ConnectToRequest(memoryBit, -0.5f),
                new ConnectToRequest(memoryShouldStoreMult, 1f),
            });
            valueToStoreFrom.ConnectTo(new[]
            {
                new ConnectToRequest(memoryShouldStoreMult, 1f),
            });
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            foreach (ConnectToRequest outputConn in outputConns)
            {
                SynapseFactory.CreateSynapse(memoryBit, outputConn.Neuron, outputConn.SynapseStrength);
            }
        }
    }
}
