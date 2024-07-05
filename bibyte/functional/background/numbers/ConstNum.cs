using bibyte.functional.background;
using Bibyte.neural;
using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.values
{
    /// <summary>
    /// A constant number that does not change.
    /// </summary>
    public class ConstNum : Number
    {
        private float value;

        public ConstNum(float value)
        {
            validateFloat(value);
            this.value = value;
        }

        public float GetValue() { return value; }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            IEnumerable<ConnectToRequest> connsWithNetScalars = multiplyAllConnsBy(outputConns, value);

            foreach (ConnectToRequest conn in connsWithNetScalars)
            {
                if (BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MAX
                    * BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MAX < Math.Abs(conn.SynapseStrength))
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            connectAndHandleLargeScalars(NeuronFactory.GetConst(), connsWithNetScalars);
        }
    }
}
