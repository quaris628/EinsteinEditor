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

        public override void AddSynapsesTo(Neuron output)
        {
            Neuron guassian = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "ValEqualsVal");
            (left * 100f).AddSynapsesTo(guassian);
            (right * -100f).AddSynapsesTo(guassian);

            Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValEqualsVal");
            SynapseFactory.CreateSynapse(guassian, latch, 100);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -99);

            SynapseFactory.CreateSynapse(latch, output, 1);
        }
    }
}
