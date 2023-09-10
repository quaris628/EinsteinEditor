﻿using Einstein;
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
        private static int hiddenNeuronIndex = BibiteVersionConfig.HIDDEN_NODES_INDEX_MIN;

        /// <summary>
        /// Creates a new hidden neuron.
        /// Use this instead of new Neuron(...), because this automatically sets the neuron's index.
        /// </summary>
        /// <param name="type">Neuron type (e.g. Linear, Gaussian...)</param>
        /// <returns>A new hidden neuron.</returns>
        public static Neuron CreateNeuron(NeuronType type)
        {
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
        public static Neuron CreateNeuron(NeuronType type, string descriptionPrefix)
        {
            if (type == NeuronType.Input)
            {
            throw new ArgumentException("You tried to create a new input neuron. That's not allowed, silly!");
            }
            return new Neuron(hiddenNeuronIndex, type, descriptionPrefix + hiddenNeuronIndex++);
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
        public static Neuron CreateNeuron(NeuronType type, string descriptionPrefix,
            float initialValue, float initialLastInput, float initialLastOutput)
        {
            if (type == NeuronType.Input)
            {
                throw new ArgumentException("You tried to create a new input neuron. That's not allowed, silly!");
            }
            return new Neuron(hiddenNeuronIndex, type, descriptionPrefix + hiddenNeuronIndex++,
                initialValue, initialLastInput, initialLastOutput);
        }
    }
}
