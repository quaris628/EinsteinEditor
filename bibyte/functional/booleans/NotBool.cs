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

        public override void AddSynapsesTo(Neuron output)
        {
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Not");
            boolean.AddSynapsesTo(linear);
            SynapseFactory.CreateSynapse(linear, output, -1);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, output, 1);
        }
    }
}
