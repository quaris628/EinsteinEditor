using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.circuits
{
    public abstract class Circuit
    {
        protected Synapse[] synapses;

        public Circuit()
        {
            synapses = null;
        }

        public Synapse[] GetSynapses() { return synapses; }
    }
}
