﻿using bibyte.functional.background;
using Bibyte.functional.background;
using Bibyte.functional.background.values;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BibyteTests.numbers
{
    [TestClass]
    public class ProductNumTest
    {
        [TestMethod]
        public void ToNonMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            ProductNum num = new ProductNum(left, right);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { left.GetInputNeuron(), right.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other hidden neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a mult
            Neuron hiddenMult = null;
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Mult, neuron.Type);
                hiddenMult = neuron;
            }

            // each input has a synapse of strength 1 to the hidden mult neuron
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), hiddenMult).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), hiddenMult).Strength);
            // the hidden mult neuron connects to the output mult (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hiddenMult, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(3, brain.Synapses.Count);
        }

        [TestMethod]
        public void ToMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            Neuron output = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            ProductNum num = new ProductNum(left, right);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Assert.IsTrue(brain.ContainsNeuron(left.GetInputNeuron()));
            Assert.IsTrue(brain.ContainsNeuron(right.GetInputNeuron()));
            Assert.IsTrue(brain.ContainsNeuron(output));
            // and there are no other neurons
            Assert.AreEqual(3, brain.Neurons.Count);

            // each input has a synapse of strength 1 to the output
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), output).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }

        [TestMethod]
        public void ToMultAndNonMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            Neuron nonMultOutput = Outputs.PHERE_OUT_1;
            Neuron multOutput = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            ProductNum num = new ProductNum(left, right);
            ValueConnectionTester.ConnectValueTo(num, new[] { nonMultOutput, multOutput });

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { left.GetInputNeuron(), right.GetInputNeuron(),
                nonMultOutput, multOutput };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other hidden neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a mult
            Neuron hiddenMult = null;
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Mult, neuron.Type);
                hiddenMult = neuron;
            }

            // each input has a synapse of strength 1 to the hidden mult neuron
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), hiddenMult).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), hiddenMult).Strength);
            // the hidden mult neuron connects to both outputs (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hiddenMult, nonMultOutput).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(hiddenMult, multOutput).Strength);
            // and there are no other synapses
            Assert.AreEqual(4, brain.Synapses.Count);
        }

        // also test connection to mult then nonmult or vice versa?
        // i.e. not in the same ConnectTo call
    }
}