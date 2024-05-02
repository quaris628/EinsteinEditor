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
    public class ConstNumTest
    {
        public ConstNumTest()
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
            float constant = 2.3f;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            ConstNum num = new ConstNum(constant);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is the const
            foreach (JsonNeuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(Inputs0_5.CONSTANT, neuron);
            }

            // there's a synapse of strength 2.3 to the output neuron
            Assert.AreEqual(constant, brain.GetSynapse(Inputs0_5.CONSTANT, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        // could test that there's no synapse when constant is 1 and connecting to a mult
        // or when constant is 0 and connecting to a linear
    }
}
