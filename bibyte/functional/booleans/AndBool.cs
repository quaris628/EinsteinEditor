using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class AndBool : Bool
    {
        private Bool left;
        private Bool right;

        public AndBool(Bool left, Bool right)
        {
            this.left = left;
            this.right = right;
        }

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
