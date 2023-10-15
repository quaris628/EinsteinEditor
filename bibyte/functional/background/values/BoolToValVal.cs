using bibyte.functional.background;
using Bibyte.functional.background.booleans;
using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    public class BoolToValVal : Value
    {
        public Bool Input { get; private set; }
        public BoolToValVal(Bool input)
        {
            Input = input;
        }
        protected internal override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            LinkedList<ConnectToRequest> outputConns = new LinkedList<ConnectToRequest>();
            foreach (Neuron output in outputs)
            {
                outputConns.AddLast(new ConnectToRequest(output, 1f));
            }
            Input.ConnectTo(outputConns);
        }
    }
}
