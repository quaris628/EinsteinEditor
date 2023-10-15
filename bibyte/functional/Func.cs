using Bibyte.functional.background.booleans;
using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    public class Func
    {

        // misc logic

        public static Number If(Bool condition, Number ifTrue, Number ifFalse)
        {
            return new BoolToNumNum(condition) * ifTrue + new BoolToNumNum(!(condition)) * ifFalse;
        }

        // non-functions (move to another class eventually?)

        public static Bool Rising(Bool boolean)
        {
            return new RisingBool(boolean);
        }
        public static Number Differential(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Differential);
        }

        // math

        public static Number Min(Number val1, Number val2)
        {
            return If(val1 < val2, val1, val2);
        }
        public static Number Max(Number val1, Number val2)
        {
            return If(val1 > val2, val1, val2);
        }
        public static Number Abs(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Abs);
        }
        public static Number ReLu(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.ReLu);
        }
        public static Number Sigmoid(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Sigmoid);
        }
        public static Number TanH(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.TanH);
        }
        /// <summary>
        /// The fake gaussian used in the bibites. 1/(1 + x^2).
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Number Gauss(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Gaussian);
        }

        public const float TAU = 6.28318530717f;
        public static Number Sin(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Sine);
        }
        public static Number Cos(Number val)
        {
            return new HiddenNeuronNum(val + TAU * 0.25f, NeuronType.Sine);
        }
        public static Number Tan(Number val)
        {
            return Func.Sin(val) / Func.Cos(val);
        }
        // This is an approximation of Arctan, so it is not fully precise. The equation used is here, https://www.desmos.com/calculator/vbnohv5rcd
        public static Number Arctan(Number val)
        {
            return TanH((val / (Abs(0.23f * val) + 1)) / (float)(0.5 * Math.PI)) * (float)(0.5 * Math.PI);
        }
    }
}
