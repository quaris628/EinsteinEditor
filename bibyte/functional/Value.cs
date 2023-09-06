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
    public abstract class Value
    {
        public abstract Synapse[] GetSynapsesTo(Neuron output);

        public static Value operator +(Value left, Value right)
        {
            SumVal sum = new SumVal();
            sum.Add(left);
            sum.Add(right);
            return sum;
        }

        public static Value operator -(Value value)
        {
            return new ScalaredVal(value, -1);
        }
        public static Value operator *(float scalar, Value value)
        {
            return new ScalaredVal(value, scalar);
        }
        public static Value operator *(Value value, float scalar)
        {
            return new ScalaredVal(value, scalar);
        }

        public static Value operator -(Value left, Value right)
        {
            return left + -right;
        }

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
