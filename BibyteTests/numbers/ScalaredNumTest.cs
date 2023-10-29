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
    public class ScalaredNumTest
    {
        [TestMethod]
        public void TwoPointThree()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar = 2.3f;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            ScalaredNum num = new ScalaredNum(input, scalar);
            ValueConnectionTester.ConnectValueTo(num, output);

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

            // there's a synapse of strength 2.3 to the output neuron
            Assert.AreEqual(2.3f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        [TestMethod]
        public void ThreeHundred()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar = 300f;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            ScalaredNum num = new ScalaredNum(input, scalar);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { input.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a linear
            Neuron linear = null;
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Linear, neuron.Type);
                linear = neuron;
            }

            // there's a synapse of strength 100 from the input to the linear
            Assert.AreEqual(100f, brain.GetSynapse(input.GetInputNeuron(), linear).Strength);
            // there's a synapse of strength 3 from the linear to the output
            Assert.AreEqual(3f, brain.GetSynapse(linear, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }


        [TestMethod]
        public void ThreeHundredDividedByFive()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.GREEN_BIBITE;
            float scalar1 = 300f;
            float scalar2 = 0.2f;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            ScalaredNum num1 = new ScalaredNum(input, scalar1);
            ScalaredNum num2 = new ScalaredNum(num1, scalar2);
            ValueConnectionTester.ConnectValueTo(num2, output);

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

            // there's a synapse of strength 60 to the output neuron
            Assert.AreEqual(60f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        // could test that there's no synapse when constant is 1 and connecting to a mult
        // or when constant is 0 and connecting to a linear
    }
}
