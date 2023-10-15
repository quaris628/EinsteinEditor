using Bibyte.neural;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
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

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            if (val is InputVal inputVal)
            {
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(inputVal.GetInputNeuron(), output, scalar);
                }
                return;
            }
            else if (val is ConstVal constVal)
            {
                float synapseStrength = constVal.GetValue() * scalar;
                if (synapseStrength < BibiteVersionConfig.SYNAPSE_STRENGTH_MAX
                    && BibiteVersionConfig.SYNAPSE_STRENGTH_MIN < synapseStrength)
                {
                    foreach (Neuron output in outputs)
                    {
                        SynapseFactory.CreateSynapse(Inputs.CONSTANT, output, scalar);
                    }
                    return;
                }
            }
            // connect the synapses to a linear, and create a new synapse from that linear and scale it
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Scalar");
            val.ConnectTo(new[] { linear });
            foreach (Neuron output in outputs)
            {
                SynapseFactory.CreateSynapse(linear, output, scalar);
            }
        }
    }
}
