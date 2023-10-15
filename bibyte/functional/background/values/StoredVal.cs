using Bibyte.functional;
using Bibyte.functional.background.booleans;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    public class StoredValue : Value
    {
        private Neuron loop;

        public StoredValue(Bool shouldStore, Value toStore) : this(shouldStore, toStore, 0f) { }
        public StoredValue(Bool shouldStore, Value toStore, float initialValue)
        {
            validateFloat(initialValue);
            Bool resetBool = new RisingBool(shouldStore);
            Neuron resetNeuron = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueReset");
            resetBool.ConnectTo(new[] { resetNeuron });
            // the rising bool induces a 2-tick delay (since it has 2 extra synapses)
            // so delay the input value by the same amount
            //Neuron delayLin1 = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueDelay");
            //Neuron delayLin2 = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueDelay");

            // TODO right now all other circuits don't have consistent timing anyway,
            // so it doesn't matter if we synchronize the input and the shouldStore bool

            Neuron multIn = NeuronFactory.CreateNeuron(NeuronType.Mult, "StoredValueInput");
            Neuron multReset = NeuronFactory.CreateNeuron(NeuronType.Mult, "StoredValueReset");
            loop = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueLoop",
                initialValue, initialValue, initialValue);
            toStore.ConnectTo(new[] { multIn });
            SynapseFactory.CreateSynapse(resetNeuron, multIn, 1);
            SynapseFactory.CreateSynapse(resetNeuron, multReset, 1);
            SynapseFactory.CreateSynapse(multIn, loop, 1);
            SynapseFactory.CreateSynapse(loop, loop, 1);
            SynapseFactory.CreateSynapse(loop, multReset, -1);
            SynapseFactory.CreateSynapse(multReset, loop, 1);
        }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            foreach (Neuron output in outputs)
            {
                SynapseFactory.CreateSynapse(loop, output, 1);
            }
        }
    }
}
