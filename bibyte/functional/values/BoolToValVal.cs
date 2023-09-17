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
    public class BoolToValVal : Value
    {
        public Bool Input { get; private set; }
        public BoolToValVal(Bool input)
        {
            Input = input;
        }
        public override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            Input.ConnectTo(outputs);
        }
    }
}
