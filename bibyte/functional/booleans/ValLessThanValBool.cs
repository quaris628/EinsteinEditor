using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class ValLessThanValBool : Bool
    {
        private Value left;
        private Value right;

        public ValLessThanValBool(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            // TODO figure out a circuit for this
            
            throw new NotImplementedException();
        }
    }
}
