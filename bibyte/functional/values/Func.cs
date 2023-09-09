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
        public const float TAU = 6.28318530717f;
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
        public static Value Sin(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Sine);
        }
        public static Value Cos(Value val)
        {
            return new HiddenNeuronVal(val + TAU * 0.25f, NeuronType.Sine);
        }
        public static Value Differential(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Differential);
        }
        public static Value Latch(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Latch);
        }
    }
}
