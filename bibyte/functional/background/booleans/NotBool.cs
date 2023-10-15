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
    /// A boolean that returns true if its input boolean is false,
    ///   and returns false if its input boolean is true.
    /// </summary>
    public class NotBool : Bool
    {
        private Bool boolean;

        public NotBool(Bool boolean)
        {
            this.boolean = boolean;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            LinkedList<ConnectToRequest> connectionsFromBoolean = new LinkedList<ConnectToRequest>();
            if (containsMults(outputConns))
            {
                Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "Not");
                connectionsFromBoolean.AddLast(new ConnectToRequest(latch, -3f));
                SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, 2);
                foreach (ConnectToRequest conn in outputConns)
                {
                    if (conn.Neuron.Type == NeuronType.Mult)
                    {
                        SynapseFactory.CreateSynapse(latch, conn.Neuron, conn.SynapseStrength);
                    }
                    else
                    {
                        connectionsFromBoolean.AddLast(new ConnectToRequest(conn.Neuron, -conn.SynapseStrength));
                        SynapseFactory.CreateSynapse(Inputs.CONSTANT, conn.Neuron, conn.SynapseStrength);
                    }
                }
            }
            else
            {
                foreach (ConnectToRequest conn in outputConns)
                {
                    connectionsFromBoolean.AddLast(new ConnectToRequest(conn.Neuron, -conn.SynapseStrength));
                    SynapseFactory.CreateSynapse(Inputs.CONSTANT, conn.Neuron, conn.SynapseStrength);
                }
            }
            boolean.ConnectTo(connectionsFromBoolean);
        }
    }
}
