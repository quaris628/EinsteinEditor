using Bibyte.functional.values;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
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
    }
}
