using Einstein;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.neural
{
    public class SynapseFactory
    {
        /// <summary>
        /// Creates a new synapse.
        /// Use this instead of new Synapse(...), because this automatically
        ///     adds the synapse and its neurons to the brain.
        /// </summary>
        /// <param name="from">Neuron the synapse starts from</param>
        /// <param name="to">Neuron the synapse goes to</param>
        /// <param name="strength">The synapse's strength</param>
        /// <returns>A new synapse</returns>
        public static JsonSynapse CreateSynapse(JsonNeuron from, JsonNeuron to, float strength)
        {
            if (strength < BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MIN
                || BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MAX < strength)
            {
                throw new ArgumentException(
                    "Invalid synapse strength '"
                    + strength
                    + "'. Must be between -100 and 100");
            }
            JsonSynapse synapse = new JsonSynapse(from, to, strength);
            NeuralBackgroundBrainBuilder.AddToBrain(synapse);
            return synapse;
        }
    }
}
