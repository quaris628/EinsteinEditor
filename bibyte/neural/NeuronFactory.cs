using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.neural
{
    public static class NeuronFactory
    {
        private static int hiddenNeuronIndex = int.MinValue;// BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN;
        private static BibiteVersion bibiteVersion = null;

        public static void InitializeBibiteVersion(BibiteVersion bibiteVersion)
        {
            if (NeuronFactory.bibiteVersion != null)
            {
                throw new InvalidOperationException("A bibite version has already been initialized");
            }

            NeuronFactory.bibiteVersion = bibiteVersion;
            hiddenNeuronIndex = bibiteVersion.HIDDEN_NODES_INDEX_MIN;
        }

        public static BibiteVersion GetBibiteVersion()
        {
            return bibiteVersion;
        }

        public static JsonNeuron GetConst()
        {
            if (bibiteVersion == null)
            {
                throw new InvalidOperationException("Must initialize bibite version before creating a neuron");
            }
            if (bibiteVersion.Equals(BibiteVersion.V0_5))
            {
                return Inputs0_5.CONSTANT;
            }
            if (bibiteVersion.Equals(BibiteVersion.V0_6_0a))
            {
                return Inputs0_6_0a.CONSTANT;
            }
            throw new NoSuchVersionException($"Bibite version '{bibiteVersion.VERSION_NAME}' does not have a Constant neuron defined");
        }

        /// <summary>
        /// Creates a new hidden neuron.
        /// Use this instead of new Neuron(...), because this automatically sets the neuron's index.
        /// </summary>
        /// <param name="type">Neuron type (e.g. Linear, Gaussian...)</param>
        /// <returns>A new hidden neuron.</returns>
        public static JsonNeuron CreateNeuron(NeuronType type)
        {
            if (bibiteVersion == null)
            {
                throw new InvalidOperationException("Must initialize bibite version before creating a neuron");
            }
            return CreateNeuron(type, "Hidden");
        }

        /// <summary>
        /// Creates a new hidden neuron.
        /// Use this instead of new Neuron(...), because this automatically sets the neuron's index.
        /// </summary>
        /// <param name="type">Neuron type (e.g. Linear, Gaussian...)</param>
        /// <param name="descriptionPrefix">Text that the neuron description should start with.
        ///   To keep descriptions unique, each description automatically has its index appended to the end.</param>
        /// <returns>A new hidden neuron.</returns>
        public static JsonNeuron CreateNeuron(NeuronType type, string descriptionPrefix)
        {
            if (bibiteVersion == null)
            {
                throw new InvalidOperationException("Must initialize bibite version before creating a neuron");
            }
            if (type == NeuronType.Input)
            {
            throw new ArgumentException("You tried to create a new input neuron. That's not allowed, silly!");
            }
            return new JsonNeuron(hiddenNeuronIndex, type, descriptionPrefix + hiddenNeuronIndex++, bibiteVersion);
        }

        /// <summary>
        /// Creates a new hidden neuron.
        /// Use this instead of new Neuron(...), because this automatically sets the neuron's index.
        /// </summary>
        /// <param name="type">Neuron type (e.g. Linear, Gaussian...)</param>
        /// <param name="descriptionPrefix">Text that the neuron description should start with.
        ///   To keep descriptions unique, each description automatically has its index appended to the end.</param>
        /// <param name="initialValue"></param>
        /// <param name="initialLastInput"></param>
        /// <param name="initialLastOutput"></param>
        /// <returns></returns>
        public static JsonNeuron CreateNeuron(NeuronType type, string descriptionPrefix,
            float initialValue, float initialLastInput, float initialLastOutput)
        {
            if (bibiteVersion == null)
            {
                throw new InvalidOperationException("Must initialize bibite version before creating a neuron");
            }
            if (type == NeuronType.Input)
            {
                throw new ArgumentException("You tried to create a new input neuron. That's not allowed, silly!");
            }
            return new JsonNeuron(hiddenNeuronIndex, type, descriptionPrefix + hiddenNeuronIndex++,
                initialValue, initialLastInput, initialLastOutput, bibiteVersion);
        }
    }
}
