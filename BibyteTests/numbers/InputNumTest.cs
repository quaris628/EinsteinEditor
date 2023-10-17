using bibyte.functional.background;
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
    public class InputNumTest
    {
        [TestMethod]
        public void InputToOutput()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            InputNum input = InputNum.PHERO_SENSE_1;
            ValueConnectionTester.ConnectValueTo(input, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { input.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's no other neurons
            Assert.AreEqual(ioNeurons.Length, brain.Neurons.Count);

            // there's a synapse of strength 1 to the output neuron
            Assert.AreEqual(1f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }
    }
}
