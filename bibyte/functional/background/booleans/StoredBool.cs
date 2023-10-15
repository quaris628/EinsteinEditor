using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bibyte.neural;
using Einstein.model;
using Bibyte.functional.background.values;
using bibyte.functional.background;

namespace Bibyte.functional.background.booleans
{
    public class StoredBool : Bool
    {
        private Neuron memoryBit;
        public StoredBool(Bool shouldStore, Bool valueToStoreFrom)
            : this(shouldStore, valueToStoreFrom, false) { }
        public StoredBool(Bool shouldStore, Bool valueToStoreFrom, bool initialValue)
        {
            float initialValueFloat = initialValue ? 1f : 0f;
            Neuron memoryShouldStoreMult = NeuronFactory.CreateNeuron(NeuronType.Mult, "memoryShouldStoreMult");

            Neuron memoryBit = NeuronFactory.CreateNeuron(NeuronType.Latch, "memoryBit",
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
