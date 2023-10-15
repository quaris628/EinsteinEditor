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
    /// A constant boolean. Always true or always false.
    /// </summary>
    public class ConstBool : Bool
    {
        private bool boolean;

        public ConstBool(bool boolean)
        {
            this.boolean = boolean;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            foreach (ConnectToRequest outputConn in outputConns)
            {
                if (outputConn.Neuron.Type == NeuronType.Mult && boolean
                    || outputConn.Neuron.Type != NeuronType.Mult && !boolean)
                {
                    continue;
                }
                // even more optimal would be to remove these output neurons from the brain
                // but that's hard to do with the current class and methods design
                // and using constant booleans will probably be rare
                // so just do this (at least for now)
                SynapseFactory.CreateSynapse(
                    Inputs.CONSTANT,
                    outputConn.Neuron,
                    boolean ? outputConn.SynapseStrength : 0);
            }
        }
    }
}
