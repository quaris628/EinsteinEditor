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
    public class HiddenNeuronVal : Value
    {
        private Neuron hidden;
        public HiddenNeuronVal(Value input, NeuronType type)
        {
            this.hidden = NeuronFactory.CreateNeuron(type, type.ToString());
            input.ConnectTo(new[] { hidden });
        }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            foreach (Neuron output in outputs)
            {
                SynapseFactory.CreateSynapse(hidden, output, 1f);
            }
        }
    }
}
