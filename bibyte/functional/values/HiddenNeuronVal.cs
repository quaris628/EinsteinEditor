using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    public class HiddenNeuronVal : Value
    {
        private Value input;
        private NeuronType type;
        public HiddenNeuronVal(Value input, NeuronType type)
        {
            this.input = input;
            this.type = type;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            Neuron hidden = NeuronFactory.CreateNeuron(type, type.ToString());
            input.AddSynapsesTo(hidden);
            SynapseFactory.CreateSynapse(hidden, output, 1);
        }
    }
}
