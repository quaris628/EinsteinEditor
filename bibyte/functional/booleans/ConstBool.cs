using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class ConstBool : Bool
    {
        private bool boolean;

        public ConstBool(bool boolean)
        {
            this.boolean = boolean;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            // TODO figure out a circuit for this

            throw new NotImplementedException();
        }
    }
}
