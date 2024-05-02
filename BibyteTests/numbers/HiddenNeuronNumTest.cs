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
    public class HiddenNeuronNumTest
    {
        public HiddenNeuronNumTest()
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
        public void Sine()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.RED_BIBITE;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            HiddenNeuronNum num = new HiddenNeuronNum(input, NeuronType.Sine);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { input.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a sine
            JsonNeuron hidden = null;
            foreach (JsonNeuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Sine, neuron.Type);
                hidden = neuron;
            }

            // the input has a synapse of strength 1 to the hidden neuron
            Assert.AreEqual(1f, brain.GetSynapse(input.GetInputNeuron(), hidden).Strength);
            // the hidden neuron connects to the output mult (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hidden, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }

        [TestMethod]
        public void Gaussian()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.RED_BIBITE;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            HiddenNeuronNum num = new HiddenNeuronNum(input, NeuronType.Gaussian);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { input.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a gaussian
            JsonNeuron hidden = null;
            foreach (JsonNeuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Gaussian, neuron.Type);
                hidden = neuron;
            }

            // the input has a synapse of strength 1 to the hidden neuron
            Assert.AreEqual(1f, brain.GetSynapse(input.GetInputNeuron(), hidden).Strength);
            // the hidden neuron connects to the output mult (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hidden, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InputThrowsException()
        {
            // Arrange
            InputNum input = InputNum.RED_BIBITE;

            // Act
            new HiddenNeuronNum(input, NeuronType.Input);
        }

        // could test more neuron types
    }
}
