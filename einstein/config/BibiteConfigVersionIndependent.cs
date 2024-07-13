using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein
{
    public struct BibiteConfigVersionIndependent
    {
        public const float SYNAPSE_STRENGTH_MIN = float.MinValue;
        public const float SYNAPSE_STRENGTH_MAX = float.MaxValue;
        public const int SYNAPSE_STRENGTH_MAX_DECIMALS = 4;
        public const float NEURON_BIAS_MIN = float.MinValue;
        public const float NEURON_BIAS_MAX = float.MaxValue;
        public const int NEURON_BIAS_MAX_DECIMALS = 4;
    }
}
