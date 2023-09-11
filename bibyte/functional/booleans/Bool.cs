using bibyte.functional.background;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public abstract class Bool : Neuralizeable
    {
        public static Bool operator ==(Bool left, Bool right)
        {
            return (left & right) | (!left & !right);
        }

        public static Bool operator !=(Bool left, Bool right)
        {
            return !(left == right);
        }

        // can't use && or ||, but & and | are available
        public static Bool operator &(Bool left, Bool right)
        {
            return new AndBool(left, right);
        }

        public static Bool operator |(Bool left, Bool right)
        {
            return !(!left & !right);
        }

        public static Bool operator !(Bool boolean)
        {
            return new NotBool(boolean);
        }

        protected override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            ConnectTo(outputs, 1f);
        }
        protected abstract void ConnectTo(IEnumerable<Neuron> outputs, float outputSynapseStrengthOverride);
    }
}
