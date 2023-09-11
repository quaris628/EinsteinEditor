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
    public class AndBool : Bool
    {
        private Bool left;
        private Bool right;

        public AndBool(Bool left, Bool right)
        {
            this.left = left;
            this.right = right;
        }

        public override void ConnectTo(Neuron output, float outputSynapseStrengthOverride)
        {
            if (output.Type == NeuronType.Mult)
            {
                left.AddOutput(output);
                right.AddOutput(output);
            }
            else
            {
                Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "And");
                left.AddOutput(mult);
                right.AddOutput(mult);
                SynapseFactory.CreateSynapse(mult, output, 1);
            }
        }
    }
}
