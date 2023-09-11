using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.functional.values;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bibyte.neural;
using Einstein.model;

namespace Bibyte.functional.memory
{
    public class StoredBool : Bool
    {
        private Bool shouldStore;
        private Bool valueToStoreFrom;
        private float initialValue;
        public StoredBool(Bool shouldStore, Bool valueToStoreFrom)
            : this(shouldStore, valueToStoreFrom, false) { }
        public StoredBool(Bool shouldStore, Bool valueToStoreFrom, bool initialValue)
        {
            this.shouldStore = shouldStore;
            this.valueToStoreFrom = valueToStoreFrom;
            this.initialValue = initialValue ? 1f : 0f;
        }

        public override void ConnectTo(Neuron output, float outputSynapseStrengthOverride)
        {
            Neuron memoryNeuron = NeuronFactory.CreateNeuron(NeuronType.Latch, "memoryBit",
                initialValue, initialValue, initialValue);
            Neuron shouldStoreNeuron = NeuronFactory.CreateNeuron(NeuronType.Linear, "shouldStoreInput");
            Value inputGate = (new BoolToValVal(shouldStore)) * (new BoolToValVal(valueToStoreFrom));
            shouldStore.AddOutput(shouldStoreNeuron);
            inputGate.AddOutput(memoryNeuron);
            (new ConstVal(0.5f)).AddOutputSynapse(memoryNeuron);
            SynapseFactory.CreateSynapse(shouldStoreNeuron, memoryNeuron, -0.5f);
            SynapseFactory.CreateSynapse(memoryNeuron, output, outputSynapseStrengthOverride);
        }
    }
}
