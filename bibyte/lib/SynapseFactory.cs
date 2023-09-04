using Einstein;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.lib
{
    public class SynapseFactory
    {
        /// <summary>
        /// Creates a new synapse.
        /// Use this instead of new Synapse(...), because this automatically
        ///     adds the neuron and its synapses to the brain.
        /// </summary>
        /// <param name="from">Neuron the synapse starts from</param>
        /// <param name="to">Neuron the synapse goes to</param>
        /// <param name="strength">The synapse's strength</param>
        /// <returns>A new synapse</returns>
        public static Synapse CreateSynapse(Neuron from, Neuron to, float strength)
        {
            if (strength < BibiteVersionConfig.SYNAPSE_STRENGTH_MIN
                || BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < strength)
            {
                throw new ArgumentException(
                    "Invalid synapse strength '"
                    + strength
                    + "'. Must be between -100 and 100");
            }
            Synapse synapse = new Synapse(from, to, strength);
            BackgroundBrainBuilder.AddToBrain(synapse);
            return synapse;
        }
    }
}
