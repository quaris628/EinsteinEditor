using Bibyte.functional.booleans;
using Bibyte.functional.values;
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
    public class ToggledBool : Bool
    {
        Bool shouldToggle;
        bool initalState;
        public ToggledBool(Bool shouldToggle, bool initialState)
        {
            this.shouldToggle = shouldToggle;
            this.initalState = initialState;
        }

        public override void AddSynapsesTo(Neuron output, float outputSynapseStrengthOverride)
        {
            Neuron memoryNeuron = NeuronFactory.CreateNeuron(NeuronType.Latch, "memoryBit");
            Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "memoryGate");
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
            shouldToggle.AddSynapsesTo(mult);
            shouldToggle.AddSynapsesTo(linear);
            (new ConstVal(0.5f)).AddSynapsesTo(linear);
            SynapseFactory.CreateSynapse(memoryNeuron, mult, 1f);
            SynapseFactory.CreateSynapse(mult, linear, -2f);
            SynapseFactory.CreateSynapse(linear, memoryNeuron, 1f);
            SynapseFactory.CreateSynapse(memoryNeuron, output, outputSynapseStrengthOverride);
        }
    }
}
