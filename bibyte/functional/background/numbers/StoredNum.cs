using bibyte.functional.background;
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
    /// <summary>
    /// A number stored in memory.
    /// Takes 2 inputs:
    ///  - A boolean that, when true, will overwrite the stored number with the input number to store
    ///  - The number to be stored when the should-store boolean is true
    /// </summary>
    public class StoredNum : Number
    {
        private Neuron loop;

        public StoredNum(Bool shouldStore, Number toStore) : this(shouldStore, toStore, 0f) { }
        public StoredNum(Bool shouldStore, Number toStore, float initialValue)
        {
            validateFloat(initialValue);
            Bool resetBool = new RisingBool(shouldStore);
            Neuron resetNeuron = NeuronFactory.CreateNeuron(NeuronType.Linear, "StoredValueReset");
            resetBool.ConnectTo(new[] { new ConnectToRequest(resetNeuron, 1f) });
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
            toStore.ConnectTo(new[] { new ConnectToRequest(multIn, 1f) });
            SynapseFactory.CreateSynapse(resetNeuron, multIn, 1f);
            SynapseFactory.CreateSynapse(resetNeuron, multReset, 1f);
            SynapseFactory.CreateSynapse(multIn, loop, 1f);
            SynapseFactory.CreateSynapse(loop, loop, 1f);
            SynapseFactory.CreateSynapse(loop, multReset, -1f);
            SynapseFactory.CreateSynapse(multReset, loop, 1f);
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            connectAndHandleLargeScalars(loop, outputConns);
        }
    }
}
