using bibyte.functional.background;
using Bibyte.functional.background.booleans;
using Bibyte.functional.background.values;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background
{
    public abstract class Number : Value
    {
        public void PlugIntoOutput(Neuron outputNeuron)
        {
            if (!outputNeuron.IsOutput())
            {
                throw new ArgumentException("A value should only be plugged into an output neuron.");
            }
            ConnectTo(new[] { outputNeuron });
        }

        // math

        public static Number operator +(Number left, Number right)
        {
            return new SumNum(left, right);
        }
        public static Number operator +(Number left, float right)
        {
            return new SumNum(left, right);
        }
        public static Number operator +(float left, Number right)
        {
            return right + left;
        }

        public static Number operator -(Number value)
        {
            return new ScalaredNum(value, -1);
        }

        public static Number operator -(Number left, Number right)
        {
            return left + -right;
        }
        public static Number operator -(Number left, float right)
        {
            return left + -right;
        }
        public static Number operator -(float left, Number right)
        {
            return left + -right;
        }

        public static Number operator *(float scalar, Number value)
        {
            return new ScalaredNum(value, scalar);
        }
        public static Number operator *(Number value, float scalar)
        {
            return new ScalaredNum(value, scalar);
        }
        public static Number operator *(Number left, Number right)
        {
            return new ProductNum(left, right);
        }

        /// <summary>
        /// This is only an approximation of division and breaks when the denominator is near zero.
        /// The error is less than 1% when the denominator is farther than 0.1 from zero,
        /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Number operator /(Number numerator, Number denominator)
        {
            return numerator * new InverseNum(denominator);
        }

        /// <summary>
        /// This is only an approximation of division and breaks when the denominator is near zero.
        /// The error is less than 1% when the denominator is farther than 0.1 from zero,
        /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Number operator /(float numerator, Number denominator)
        {
            return numerator * new InverseNum(denominator);
        }
        public static Number operator /(Number numerator, float denominator)
        {
            return numerator * (1 / denominator);
        }

        public static Number operator ^(double baseVal, Number exponent)
        {
            return 1 / new HiddenNeuronNum(-exponent * (float)Math.Log(baseVal), NeuronType.Sigmoid) + new ConstNum(-1);
        }

        // value-value comparisons

        /// <summary>
        /// This is only an approximation of an equals.
        /// Once the values exactly match, this will return true until the values are
        /// more than about 1x10^-5 i.e. 0.00001 away from each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Bool operator ==(Number left, Number right)
        {
            return new ValEqualsValBool(left, right);
        }
        public static Bool operator !=(Number left, Number right)
        {
            return !(left == right);
        }
        public static Bool operator <(Number left, Number right)
        {
            return new ValLessThanValBool(left, right);
        }
        public static Bool operator >(Number left, Number right)
        {
            return right < left;
        }
        public static Bool operator <=(Number left, Number right)
        {
            return new ValLessThanEqValBool(left, right);
        }
        public static Bool operator >=(Number left, Number right)
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
        public static Bool operator ==(Number left, float right)
        {
            return new ValEqualsValBool(left, new ConstNum(right));
        }
        public static Bool operator !=(Number left, float right)
        {
            return !(left == right);
        }
        public static Bool operator <(Number left, float right)
        {
            return new ValLessThanValBool(left, new ConstNum(right));
        }
        public static Bool operator >(Number left, float right)
        {
            return right < left;
        }
        public static Bool operator <=(Number left, float right)
        {
            return new ValLessThanEqValBool(left, new ConstNum(right));
        }
        public static Bool operator >=(Number left, float right)
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
        public static Bool operator ==(float left, Number right)
        {
            return new ValEqualsValBool(new ConstNum(left), right);
        }
        public static Bool operator !=(float left, Number right)
        {
            return !(left == right);
        }
        public static Bool operator <(float left, Number right)
        {
            return new ValLessThanValBool(new ConstNum(left), right);
        }
        public static Bool operator >(float left, Number right)
        {
            return right < left;
        }
        public static Bool operator <=(float left, Number right)
        {
            return new ValLessThanEqValBool(new ConstNum(left), right);
        }
        public static Bool operator >=(float left, Number right)
        {
            return right <= left;
        }

        public static implicit operator Number(float scalar)
        {
            return new ConstNum(scalar);
        }
        public static explicit operator Number(Bool boolean)
        {
            return new BoolToNumNum(boolean);
        }
    }
}
