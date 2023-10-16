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

        public static void ConnectValueTo(Value val, Neuron outputNeuron)
        {
            ConnectValueTo(val, new Neuron[] { outputNeuron });
        }
        
        public static void ConnectValueTo(Value val, IEnumerable<Neuron> outputNeurons)
        {
            val.ConnectTo(outputNeurons);
        }
    }
}
