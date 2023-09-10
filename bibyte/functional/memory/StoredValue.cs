using bibyte.functional.booleans;
using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.memory
{
    public class StoredValue : Value
    {
        private Bool shouldStore;
        private Value toStore;
        public StoredValue(Bool shouldStore, Value toStore)
        {
            this.shouldStore = shouldStore;
            this.toStore = toStore;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            Bool resetBool = new RisingBool(shouldStore);
            Neuron resetNeuron = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueReset");
            resetBool.AddSynapsesTo(resetNeuron);
            // the rising bool induces a 2-tick delay (since it has 2 extra synapses)
            // so delay the input value by the same amount
            //Neuron delayLin1 = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueDelay");
            //Neuron delayLin2 = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueDelay");

            // TODO right now all other circuits don't have consistent timing anyway,
            // so it doesn't matter if we synchronize the input and the shouldStore bool

            Neuron multIn = NeuronFactory.CreateNeuron(NeuronType.Mult, "StoredValueInput");
            Neuron multReset = NeuronFactory.CreateNeuron(NeuronType.Mult, "StoredValueReset");
            Neuron loop = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueLoop");
            toStore.AddSynapsesTo(multIn);
            SynapseFactory.CreateSynapse(resetNeuron, multIn, 1);
            SynapseFactory.CreateSynapse(resetNeuron, multReset, 1);
            SynapseFactory.CreateSynapse(multIn, loop, 1);
            SynapseFactory.CreateSynapse(loop, loop, 1);
            SynapseFactory.CreateSynapse(loop, multReset, -1);
            SynapseFactory.CreateSynapse(multReset, loop, 1);
            SynapseFactory.CreateSynapse(loop, output, 1);
        }
    }
}
