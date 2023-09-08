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
    public abstract class Value : INeuralizeable
    {
        // math

        public static Value operator +(Value left, Value right)
        {
            SumVal sum = new SumVal();
            sum.Add(left);
            sum.Add(right);
            return sum;
        }
        public static Value operator +(Value left, float right)
        {
            SumVal sum = new SumVal();
            sum.Add(left);
            sum.Add(new ConstVal(right));
            return sum;
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
        public static Value operator /(float numerator, Value denominator)
        {
            return numerator * new InverseVal(denominator);
        }
        public static Value operator /(Value numerator, float denominator)
        {
            return numerator * (1 / denominator);
        }

        public static Value operator ^(Value baseVal, Value exponent)
        {
            // TODO write ExponentVal class, use BibiteBoi's e^x approximation
            throw new NotImplementedException();
        }

        // value-value comparisons

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
            return new ValLessThanValBool(right, left);
        }

        public static Bool operator <=(Value left, Value right)
        {
            return !(left > right);
        }

        public static Bool operator >=(Value left, Value right)
        {
            return !(left < right);
        }


        public abstract void AddSynapsesTo(Neuron output);

        protected void validateFloat(float val)
        {
            if (val < BibiteVersionConfig.SYNAPSE_STRENGTH_MIN
            || BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < val)
            {
                throw new ArgumentException("bad value, must be between -100 and 100");
            }
        }
    }
}
