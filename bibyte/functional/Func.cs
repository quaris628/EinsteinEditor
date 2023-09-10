﻿using bibyte.functional.booleans;
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
            return new IfVal(condition, ifTrue, ifFalse);
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
    }
}