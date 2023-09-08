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
    public class ValEqualsValBool : Bool
    {
        private Value left;
        private Value right;

        public ValEqualsValBool(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            Neuron guassianNeuron = NeuronFactory.CreateNeuron(NeuronType.Gaussian);

            Neuron latchNeuron = NeuronFactory.CreateNeuron(NeuronType.Latch);
            Synapse guassianToLatch = SynapseFactory.CreateSynapse(guassianNeuron, latchNeuron, 100);
            Synapse constToLatch = SynapseFactory.CreateSynapse(Inputs.CONSTANT, latchNeuron, -99);
            Synapse[] leftSynapses = left.AddSynapsesTo(guassianNeuron);
            foreach (Synapse synapse in leftSynapses)
            {
                synapse.Strength = 100f;
            }
            Synapse[] rightSynapses = right.AddSynapsesTo(guassianNeuron);
            foreach (Synapse synapse in rightSynapses)
            {
                synapse.Strength = -100f;
            }
            return new Synapse[] { SynapseFactory.CreateSynapse(latchNeuron, output, 1) };
        }
    }
}
