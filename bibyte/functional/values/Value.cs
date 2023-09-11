using bibyte.functional.background;
using Bibyte.functional.booleans;
using Bibyte.functional.values;
using Bibyte.neural;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    // pall functions of subclasses into operator overloads
    public abstract class Value : Neuralizeable
    {
        // math

        public static Value operator +(Value left, Value right)
        {
            return new SumVal(left, right);
        }
        public static Value operator +(Value left, float right)
        {
            return new SumVal(left, right);
        }
        public static Value operator +(float left, Value right)
        {
            return right + left;
        }

        public static Value operator -(Value value)
        {
            return new ScalaredVal(value, -1);
        }

        public static Value operator -(Value left, Value right)
        {
            return left + -right;
        }
        public static Value operator -(Value left, float right)
        {
            return left + -right;
        }
        public static Value operator -(float left, Value right)
        {
            return left + -right;
        }

        public static Value operator *(float scalar, Value value)
        {
            return new ScalaredVal(value, scalar);
        }
        public static Value operator *(Value value, float scalar)
        {
            return new ScalaredVal(value, scalar);
        }
        public static Value operator *(Value left, Value right)
        {
            return new ProductVal(new List<Value> { left, right });
        }

        /// <summary>
        /// This is only an approximation of division and breaks when the denominator is near zero.
        /// The error is less than 1% when the denominator is farther than 0.1 from zero,
        /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Value operator /(Value numerator, Value denominator)
        {
            return numerator * new InverseVal(denominator);
        }

        /// <summary>
        /// This is only an approximation of division and breaks when the denominator is near zero.
        /// The error is less than 1% when the denominator is farther than 0.1 from zero,
        /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Value operator /(float numerator, Value denominator)
        {
            return numerator * new InverseVal(denominator);
        }
        public static Value operator /(Value numerator, float denominator)
        {
            return numerator * (1 / denominator);
        }

        public static Value operator ^(double baseVal, Value exponent)
        {
            return 1 / new HiddenNeuronVal(-exponent * (float)Math.Log(baseVal), NeuronType.Sigmoid) + new ConstVal(-1);
        }

        // value-value comparisons

        /// <summary>
        /// This is only an approximation of an equals.
        /// Once the values exactly match, this will return true until the values are
        /// more than about 1x10^-5 i.e. 0.00001 away from each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Bool operator ==(Value left, Value right)
        {
            return new ValEqualsValBool(left, right);
        }
        public static Bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }
        public static Bool operator <(Value left, Value right)
        {
            return new ValLessThanValBool(left, right);
        }
        public static Bool operator >(Value left, Value right)
        {
            return right < left;
        }
        public static Bool operator <=(Value left, Value right)
        {
            return new ValLessThanEqValBool(left, right);
        }
        public static Bool operator >=(Value left, Value right)
        {
            return right <= left;
        }

        // value-scalar comparisions

        /// <summary>
        /// This is only an approximation of an equals.
        /// Once the values exactly match, this will return true until the values are
        /// more than about 1x10^-5 i.e. 0.00001 away from each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Bool operator ==(Value left, float right)
        {
            return new ValEqualsValBool(left, new ConstVal(right));
        }
        public static Bool operator !=(Value left, float right)
        {
            return !(left == right);
        }
        public static Bool operator <(Value left, float right)
        {
            return new ValLessThanValBool(left, new ConstVal(right));
        }
        public static Bool operator >(Value left, float right)
        {
            return right < left;
        }
        public static Bool operator <=(Value left, float right)
        {
            return new ValLessThanEqValBool(left, new ConstVal(right));
        }
        public static Bool operator >=(Value left, float right)
        {
            return right <= left;
        }
        /// <summary>
        /// This is only an approximation of an equals.
        /// Once the values exactly match, this will return true until the values are
        /// more than about 1x10^-5 i.e. 0.00001 away from each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Bool operator ==(float left, Value right)
        {
            return new ValEqualsValBool(new ConstVal(left), right);
        }
        public static Bool operator !=(float left, Value right)
        {
            return !(left == right);
        }
        public static Bool operator <(float left, Value right)
        {
            return new ValLessThanValBool(new ConstVal(left), right);
        }
        public static Bool operator >(float left, Value right)
        {
            return right < left;
        }
        public static Bool operator <=(float left, Value right)
        {
            return new ValLessThanEqValBool(new ConstVal(left), right);
        }
        public static Bool operator >=(float left, Value right)
        {
            return right <= left;
        }

        public static implicit operator Value(float scalar)
        {
            return new ConstVal(scalar);
        }
    }
}
