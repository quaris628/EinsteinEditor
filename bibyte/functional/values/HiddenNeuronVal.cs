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
        private Neuron hidden;
        public HiddenNeuronVal(Value input, NeuronType type)
        {
            Neuron hidden = NeuronFactory.CreateNeuron(type, type.ToString());
            input.AddOutput(hidden);
        }

        public override void AddOutputSynapse(Neuron output)
        {
            SynapseFactory.CreateSynapse(hidden, output, 1);
        }
    }
}
