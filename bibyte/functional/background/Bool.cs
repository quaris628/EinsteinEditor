using bibyte.functional.background;
using Bibyte.functional.background.booleans;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background
{
    public abstract class Bool : Value
    {
        protected internal sealed override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            LinkedList<ConnectToRequest> connections =
                    new LinkedList<ConnectToRequest>();
            foreach (Neuron output in outputs)
            {
                connections.AddLast(new ConnectToRequest(output, 1f));
            }
            ConnectTo(outputs);
        }
        
        protected internal abstract void ConnectTo(IEnumerable<ConnectToRequest> outputConns);

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
    }
}
