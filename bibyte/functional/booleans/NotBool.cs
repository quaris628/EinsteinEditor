using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class NotBool : Bool
    {
        private Bool boolean;

        public NotBool(Bool boolean)
        {
            this.boolean = boolean;
        }

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
