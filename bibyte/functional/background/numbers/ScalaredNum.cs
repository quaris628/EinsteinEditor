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
    /// <summary>
    /// A number that's a scalar multiple of an input number
    /// </summary>
    public class ScalaredNum : Number
    {
        private Number val;
        private float scalar;
        private Neuron linear;

        public ScalaredNum(Number val, float scalar)
        {
            validateFloat(scalar);
            this.val = val;
            this.scalar = scalar;
            this.linear = null;
        }

        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            if (val is InputNum inputVal)
            {
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(inputVal.GetInputNeuron(), output, scalar);
                }
                return;
            }
            else if (val is ConstNum constVal)
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
            // TODO could optimize with a synapse strength override for values just like how the bools do it
            if (linear == null)
            {
                Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Scalar");
                val.ConnectTo(new[] { linear });
            }
            foreach (Neuron output in outputs)
            {
                SynapseFactory.CreateSynapse(linear, output, scalar);
            }
        }
    }
}
