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

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            Synapse[] synapses = val.GetSynapsesTo(output);
            // You can reuse synapses in more cases than they're all = 1
            // you can reuse them only if their strengths are all the same, and if
            // the multiplier you're using doesn't stack all of their weights to
            // something beyond -100 or 100.
            // But since this is much simpler and will probably work in most cases, I'll do this for now
            bool canReuseSynapses = true;
            foreach (Synapse synapse in synapses)
            {
                if (synapse.Strength != 1)
                {
                    canReuseSynapses = false;
                    break;
                }
            }
            if (canReuseSynapses)
            {
                // change the strength of the existing synapses
                foreach (Synapse synapse in synapses)
                {
                    synapse.Strength = scalar;
                }
                return synapses;
            }
            else
            {
                // connect the synapses to a linear, and create a new synapse from that linear and scale it
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
                foreach (Synapse synapse in synapses)
                {
                    synapse.To = linear;
                }
                Synapse fromLinear = SynapseFactory.CreateSynapse(linear, output, scalar);
                return synapses.Concat(new Synapse[] { fromLinear }).ToArray();
            }
        }
    }
}
