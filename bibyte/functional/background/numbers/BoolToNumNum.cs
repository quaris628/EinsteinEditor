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
        public Bool input { get; private set; }

        public BoolToNumNum(Bool input)
        {
            this.input = input;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            input.ConnectTo(outputConns);
        }
    }
}
