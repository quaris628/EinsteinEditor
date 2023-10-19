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
    public class InverseNumTest
    {
        [TestMethod]
        public void ToMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.RED_BIBITE;
            Neuron output = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            InverseNum num = new InverseNum(input);
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
            // which is a gaussian
            Neuron hidden = null;
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Gaussian, neuron.Type);
                hidden = neuron;
            }

            Assert.AreEqual(100f, brain.GetSynapse(input.GetInputNeuron(), hidden).Strength);
            Assert.AreEqual(100f, brain.GetSynapse(hidden, output).Strength);
            Assert.AreEqual(100f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            Assert.AreEqual(3, brain.Synapses.Count);
        }

        [TestMethod]
        public void ToNonMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.RED_BIBITE;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            InverseNum num = new InverseNum(input);
            ValueConnectionTester.ConnectValueTo(num, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { input.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's two other neurons
            Assert.AreEqual(ioNeurons.Length + 2, brain.Neurons.Count);
            // which are a gaussian and a mult
            Neuron hiddenGauss = null;
            Neuron hiddenMult = null;
            foreach (Neuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                if (neuron.Type == NeuronType.Gaussian)
                {
                    hiddenGauss = neuron;
                }
                if (neuron.Type == NeuronType.Mult)
                {
                    hiddenMult = neuron;
                }
            }
            Assert.IsNotNull(hiddenGauss);
            Assert.IsNotNull(hiddenMult);

            Assert.AreEqual(100f, brain.GetSynapse(input.GetInputNeuron(), hiddenGauss).Strength);
            Assert.AreEqual(100f, brain.GetSynapse(hiddenGauss, hiddenMult).Strength);
            Assert.AreEqual(100f, brain.GetSynapse(input.GetInputNeuron(), hiddenMult).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(hiddenMult, output).Strength);
            Assert.AreEqual(4, brain.Synapses.Count);
        }
    }
}
