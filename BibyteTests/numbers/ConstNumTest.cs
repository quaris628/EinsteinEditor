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
    public class ConstNumTest
    {
        [TestMethod]
        public void TwoPointThree()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            float constant = 2.3f;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            ConstNum num = new ConstNum(constant);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { output };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is the const
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(Inputs.CONSTANT, neuron);
            }

            // there's a synapse of strength 2.3 to the output neuron
            Assert.AreEqual(constant, brain.GetSynapse(Inputs.CONSTANT, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        // could test that there's no synapse when constant is 1 and connecting to a mult
        // or when constant is 0 and connecting to a linear
    }
}
