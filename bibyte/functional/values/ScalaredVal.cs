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
    public class ScalaredVal : Value
    {
        private Value val;
        private float scalar;
        public ScalaredVal(Value val, float scalar)
        {
            validateFloat(scalar);
            this.val = val;
            this.scalar = scalar;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            // connect the synapses to a linear, and create a new synapse from that linear and scale it
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
            val.AddSynapsesTo(linear);
            SynapseFactory.CreateSynapse(linear, output, scalar);
        }
    }
}
