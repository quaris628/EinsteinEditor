using bibyte.functional.booleans;
using Bibyte.functional.booleans;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    public class Func
    {

        // misc logic

        public static Value If(Bool condition, Value ifTrue, Value ifFalse)
        {
            return new BoolToValVal(condition) * ifTrue + new BoolToValVal(!(condition)) * ifFalse;
        }

        // non-functions (move to another class eventually?)

        public static Bool Rising(Bool boolean)
        {
            return new RisingBool(boolean);
        }
        public static Value Differential(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Differential);
        }

        // math

        public static Value Min(Value val1, Value val2)
        {
            return If(val1 < val2, val1, val2);
        }
        public static Value Max(Value val1, Value val2)
        {
            return If(val1 > val2, val1, val2);
        }
        public static Value Abs(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Abs);
        }
        public static Value ReLu(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.ReLu);
        }
        public static Value Sigmoid(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Sigmoid);
        }
        public static Value TanH(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.TanH);
        }
        /// <summary>
        /// The fake gaussian used in the bibites. 1/(1 + x^2).
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Value Gauss(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Gaussian);
        }

        public const float TAU = 6.28318530717f;
        public static Value Sin(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Sine);
        }
        public static Value Cos(Value val)
        {
            return new HiddenNeuronVal(val + TAU * 0.25f, NeuronType.Sine);
        }
        public static Value Tan(Value val)
        {
            return Func.Sin(val) / Func.Cos(val);
        }
        // This is an approximation of Arctan, so it is not fully precise. The equation used is here, https://www.desmos.com/calculator/vbnohv5rcd
        public static Value Arctan(Value val)
        {
            return TanH((val / (Abs(0.23f * val) + 1)) / (float)(0.5 * Math.PI)) * (float)(0.5 * Math.PI);
        }
    }
}
