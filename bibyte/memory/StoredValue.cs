using Bibyte.functional;
using Bibyte.functional.booleans;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.memory
{
    public class StoredValue : Value
    {

        public StoredValue(Bool shouldStore, Value valueToStoreFrom)
        {

        }

        public override void AddSynapsesTo(Neuron output)
        {
            // TODO
            // should probably take advantage of a loop
            // (e.g. linear <-> linear)
        }
    }
}
