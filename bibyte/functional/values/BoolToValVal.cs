using Bibyte.functional.booleans;
using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    internal class BoolToValVal : Value
    {
        Bool input;
        public BoolToValVal(Bool input)
        {
            this.input = input;
        }
        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            return input.GetSynapsesTo(output);
        }
    }
}