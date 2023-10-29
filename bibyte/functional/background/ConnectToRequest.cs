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
            Neuron = neuron;
            SynapseStrength = synapseStrength;
        }
    }
}
