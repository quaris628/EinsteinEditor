using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class NotBool : Bool
    {
        private Bool boolean;

        public NotBool(Bool boolean)
        {
            this.boolean = boolean;
        }

        public override void AddSynapsesTo(Neuron output, float outputSynapseStrengthOverride)
        {
            if (output.Type == NeuronType.Mult)
            {
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Not");
                boolean.AddSynapsesTo(linear);
                Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "Not");
                SynapseFactory.CreateSynapse(linear, latch, -3);
                SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, 2);
                SynapseFactory.CreateSynapse(latch, output, outputSynapseStrengthOverride);
            }
            else
            {
                boolean.AddSynapsesTo(output, -outputSynapseStrengthOverride);
                SynapseFactory.CreateSynapse(Inputs.CONSTANT, output, outputSynapseStrengthOverride);
            }
        }
    }
}
