using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional.background
{
    /// <summary>
    /// For unit tests only
    /// </summary>
    public class ValueConnectionTester
    {
        private ValueConnectionTester() { }

        public static void ConnectValueTo(Value val, JsonNeuron outputNeuron)
        {
            ConnectValueTo(val, new JsonNeuron[] { outputNeuron });
        }
        
        public static void ConnectValueTo(Value val, IEnumerable<JsonNeuron> outputNeurons)
        {
            LinkedList<ConnectToRequest> conns = new LinkedList<ConnectToRequest>();
            foreach (JsonNeuron outputNeuron in outputNeurons)
            {
                conns.AddLast(new ConnectToRequest(outputNeuron, 1f));
            }
            val.ConnectTo(conns);
        }
    }
}
