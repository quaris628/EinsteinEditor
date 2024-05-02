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
    public class SumNumTest
    {
        public SumNumTest()
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
        public void ToNonMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            SumNum sumNum = new SumNum(left, right);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { left.GetInputNeuron(), right.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there are no other neurons
            Assert.AreEqual(ioNeurons.Length, brain.Neurons.Count);

            // each input has a synapse of strength 1 to the output
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), output).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }

        [TestMethod]
        public void ToMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            JsonNeuron output = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            SumNum sumNum = new SumNum(left, right);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { left.GetInputNeuron(), right.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other hidden neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a linear
            JsonNeuron hiddenLinear = null;
            foreach (JsonNeuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Linear, neuron.Type);
                hiddenLinear = neuron;
            }

            // each input has a synapse of strength 1 to the hidden linear neuron
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), hiddenLinear).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), hiddenLinear).Strength);
            // the hidden linear neuron connects to the output mult (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hiddenLinear, output).Strength);
            // and there are no other synapses
            Assert.AreEqual(3, brain.Synapses.Count);
        }

        [TestMethod]
        public void ToMultAndNonMult()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            JsonNeuron nonMultOutput = Outputs0_5.PHERE_OUT_1;
            JsonNeuron multOutput = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            SumNum num = new SumNum(left, right);
            ValueConnectionTester.ConnectValueTo(num, new[] { nonMultOutput, multOutput });

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { left.GetInputNeuron(), right.GetInputNeuron(),
                nonMultOutput, multOutput };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there's one other hidden neuron
            Assert.AreEqual(ioNeurons.Length + 1, brain.Neurons.Count);
            // which is a linear
            JsonNeuron hiddenLinear = null;
            foreach (JsonNeuron neuron in brain.Neurons)
            {
                if (ioNeurons.Contains(neuron))
                {
                    continue;
                }
                Assert.AreEqual(NeuronType.Linear, neuron.Type);
                hiddenLinear = neuron;
            }

            // each input has a synapse of strength 1 to the hidden linear neuron
            Assert.AreEqual(1f, brain.GetSynapse(left.GetInputNeuron(), hiddenLinear).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(right.GetInputNeuron(), hiddenLinear).Strength);
            // the hidden linear neuron connects to both outputs (with strength 1)
            Assert.AreEqual(1f, brain.GetSynapse(hiddenLinear, nonMultOutput).Strength);
            Assert.AreEqual(1f, brain.GetSynapse(hiddenLinear, multOutput).Strength);
            // and there are no other synapses
            Assert.AreEqual(4, brain.Synapses.Count);
        }

        [TestMethod]
        public void AddToItself()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input = InputNum.GREEN_BIBITE;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            SumNum sumNum = new SumNum(input, input);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { input.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there are no other neurons
            Assert.AreEqual(ioNeurons.Length, brain.Neurons.Count);

            // there's a synapse of strength 2 to the output
            Assert.AreEqual(2f, brain.GetSynapse(input.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(1, brain.Synapses.Count);
        }

        [TestMethod]
        public void AddToItselfx3AndSomethingElse()
        {
            // Arrange
            NeuralBackgroundBrainBuilder.ClearBrain(BibiteVersion.V0_5);
            InputNum input123 = InputNum.GREEN_BIBITE;
            InputNum input4 = InputNum.RED_BIBITE;
            JsonNeuron output = Outputs0_5.PHERE_OUT_1;

            // Act
            SumNum sumNum = (SumNum)(input123 + input123 + input123 + input4);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            JsonBrain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            JsonNeuron[] ioNeurons = new JsonNeuron[] { input123.GetInputNeuron(), input4.GetInputNeuron(), output };
            foreach (JsonNeuron neuron in ioNeurons)
            {
                Assert.IsTrue(brain.ContainsNeuron(neuron));
            }
            // and there are no other neurons
            Assert.AreEqual(ioNeurons.Length, brain.Neurons.Count);

            // there's a synapse of strength 3 from input123 to the output
            Assert.AreEqual(3f, brain.GetSynapse(input123.GetInputNeuron(), output).Strength);
            // there's a synapse of strength 1 from input4 to the output
            Assert.AreEqual(1f, brain.GetSynapse(input4.GetInputNeuron(), output).Strength);
            // and there are no other synapses
            Assert.AreEqual(2, brain.Synapses.Count);
        }

        // TODO add something to itself

        // also test connection to mult then nonmult or vice versa?
        // i.e. not in the same ConnectTo call
    }
}
