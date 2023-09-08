using Bibyte.neural;
using Einstein;
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
            if (val is InputVal inputVal)
            {
                SynapseFactory.CreateSynapse(inputVal.GetInputNeuron(), output, scalar);
                return;
            }
            else if (val is ConstVal constVal)
            {
                float synapseStrength = constVal.GetValue() * scalar;
                if (synapseStrength < BibiteVersionConfig.SYNAPSE_STRENGTH_MAX
                    && BibiteVersionConfig.SYNAPSE_STRENGTH_MIN < synapseStrength)
                {
                    SynapseFactory.CreateSynapse(Inputs.CONSTANT, output, synapseStrength);
                    return;
                }
            }
            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear);
            val.AddSynapsesTo(linear);
            SynapseFactory.CreateSynapse(linear, output, scalar);
        }
    }
}
