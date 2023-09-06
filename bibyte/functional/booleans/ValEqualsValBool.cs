using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class ValEqualsValBool : Bool
    {
        private Value left;
        private Value right;

        public ValEqualsValBool(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            // TODO figure out a circuit for this

            throw new NotImplementedException();
        }
    }
}
