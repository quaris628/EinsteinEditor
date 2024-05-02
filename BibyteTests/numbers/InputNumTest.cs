using bibyte.functional.background;
using Bibyte.functional.background;
using Bibyte.functional.background.values;
using Bibyte.neural;
using Einstein.config.bibiteVersions;
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
        public InputNumTest()
        {
            try
            {
                NeuronFactory.InitializeBibiteVersion(BibiteVersion.V0_5);
            }
            catch
            {

            }
        }

        [TestMethod]
        public void InputToOutput()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            InputNum input = InputNum.PHERO_SENSE_1;
            ValueConnectionTester.ConnectValueTo(input, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { input.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
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
