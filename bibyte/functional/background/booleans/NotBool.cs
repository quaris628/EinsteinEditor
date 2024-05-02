using bibyte.functional.background;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.booleans
{
    /// <summary>
    /// A boolean that is true if its input boolean is false,
    ///   and returns false if its input boolean is true.
    /// </summary>
    public class NotBool : Bool
    {
        private Bool boolean;
        private JsonNeuron latch;

        public NotBool(Bool boolean)
        {
            this.boolean = boolean;
            this.latch = null;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            LinkedList<ConnectToRequest> fromBooleanConns = new LinkedList<ConnectToRequest>();
            if (latch == null && containsMults(outputConns))
            {
                latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "Not");
                fromBooleanConns.AddLast(new ConnectToRequest(latch, -3f));
                SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, 2);
            }

            LinkedList<ConnectToRequest> fromLatchConns = new LinkedList<ConnectToRequest>();
            LinkedList<ConnectToRequest> fromConstantConns = new LinkedList<ConnectToRequest>();
            foreach (ConnectToRequest conn in outputConns)
            {
                if (conn.Neuron.Type == NeuronType.Mult)
                {
                    fromLatchConns.AddLast(conn);
                }
                else
                {
                    fromBooleanConns.AddLast(new ConnectToRequest(conn.Neuron, -conn.SynapseStrength));
                    fromConstantConns.AddLast(conn);
                }
            }
            if (fromLatchConns.Count > 0)
            {
                connectAndHandleLargeScalars(latch, fromLatchConns);
            }
            if (fromConstantConns.Count > 0)
            {
                connectAndHandleLargeScalars(Inputs.CONSTANT, fromConstantConns);
            }
            if (fromBooleanConns.Count > 0)
            {
                boolean.ConnectTo(fromBooleanConns);
            }
        }
    }
}
