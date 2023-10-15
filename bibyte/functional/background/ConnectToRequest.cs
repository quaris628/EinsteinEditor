using Einstein;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional.background
{
    public class ConnectToRequest
    {
        public Neuron Neuron { get; private set; }
        public float SynapseStrength { get; private set; }
        public ConnectToRequest(Neuron neuron, float synapseStrength)
        {
            if (synapseStrength < BibiteVersionConfig.SYNAPSE_STRENGTH_MIN
            || BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < synapseStrength)
            {
                throw new ArgumentException(synapseStrength + " cannot be used as a synapse strength. "
                    + "Must be between -100 and 100.");
            }
            Neuron = neuron;
            SynapseStrength = synapseStrength;
        }
    }
}
