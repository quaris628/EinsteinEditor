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
    public class IfVal : Value
    {
        private Value finalOutput;
        public IfVal(Bool inputBool, Value trueValue, Value falseValue)
        {
            Value trueValueOutput = new BoolToValVal(inputBool) * trueValue;
            Value falseValueOutput = new BoolToValVal(!(inputBool)) * falseValue;
            finalOutput = trueValueOutput + falseValueOutput;
        }
        public override void AddOutputSynapse(Neuron output)
        {
            finalOutput.AddOutput(output);
        }
    }
}
