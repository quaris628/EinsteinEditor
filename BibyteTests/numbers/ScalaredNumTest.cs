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
    public class ScalaredNumTest
    {
        public ScalaredNumTest()
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
        public void TwoPointThree()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar = 2.3f;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            ScalaredNum num = new ScalaredNum(input, scalar);
            ValueConnectionTester.ConnectValueTo(num, output);

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

            // there's a synapse of strength 2.3 to the output neuron
            Assert.AreEqual(2.3f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        [TestMethod]
        public void ThreeHundred()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar = 300f;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            ScalaredNum num = new ScalaredNum(input, scalar);
            ValueConnectionTester.ConnectValueTo(num, output);

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
            
            // there's a synapse of strength 100 from the input to the output
            Assert.AreEqual(300f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }


        [TestMethod]
        public void ThreeHundredDividedByFive()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar1 = 300f;
            float scalar2 = 0.2f;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            ScalaredNum num1 = new ScalaredNum(input, scalar1);
            ScalaredNum num2 = new ScalaredNum(num1, scalar2);
            ValueConnectionTester.ConnectValueTo(num2, output);

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

            // there's a synapse of strength 60 to the output neuron
            Assert.AreEqual(60f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        // could test that there's no synapse when constant is 1 and connecting to a mult
        // or when constant is 0 and connecting to a linear
    }
}
