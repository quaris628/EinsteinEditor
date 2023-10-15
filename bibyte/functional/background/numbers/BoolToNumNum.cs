using bibyte.functional.background;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    /// <summary>
    /// Converts a boolean to a number.
    /// True converts to 1. False converts to 0.
    /// </summary>
    public class BoolToNumNum : Number
    {
        public Bool Input { get; private set; }

        public BoolToNumNum(Bool input)
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
