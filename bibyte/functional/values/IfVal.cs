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
        private Bool inputBool;
        private Value trueValue;
        private Value falseValue;
        public IfVal(Bool inputBool, Value trueValue, Value falseValue)
        {
            this.inputBool = inputBool;
            this.trueValue = trueValue;
            this.falseValue = falseValue;
        }
        public override void AddSynapsesTo(Neuron output)
        {
            Value trueValueOutput = new BoolToValVal(inputBool) * trueValue;
            Value falseValueOutput = new BoolToValVal(!(inputBool)) * falseValue;
            Value finalOutput = trueValueOutput + falseValueOutput;
            finalOutput.AddSynapsesTo(output);
        }
    }
}
