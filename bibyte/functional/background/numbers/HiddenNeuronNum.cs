using bibyte.functional.background;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    /// <summary>
    /// A number that is the function of one of the neuron types of an input number
    /// </summary>
    public class HiddenNeuronNum : Number
    {
        private JsonNeuron hidden;

        public HiddenNeuronNum(Number input, NeuronType type)
        {
            if (type == NeuronType.Input)
            {
                throw new ArgumentException("Neuron type cannot be 'Input'");
            }
            hidden = NeuronFactory.CreateNeuron(type, type.ToString());
            input.ConnectTo(new[] { new ConnectToRequest(hidden, 1f) });
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            connectAndHandleLargeScalars(hidden, outputConns);
        }
    }
}
