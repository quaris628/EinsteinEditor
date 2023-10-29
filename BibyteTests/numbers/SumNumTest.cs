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
    public class SumNumTest
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
            SumNum sumNum = new SumNum(left, right);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { left.GetInputNeuron(), right.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
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
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            Neuron output = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            SumNum sumNum = new SumNum(left, right);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

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
            // which is a linear
            Neuron hiddenLinear = null;
            foreach (Neuron neuron in brain.Neurons)
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
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum left = InputNum.GREEN_BIBITE;
            InputNum right = InputNum.RED_BIBITE;
            Neuron nonMultOutput = Outputs.PHERE_OUT_1;
            Neuron multOutput = NeuronFactory.CreateNeuron(NeuronType.Mult);

            // Act
            SumNum num = new SumNum(left, right);
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
            // which is a linear
            Neuron hiddenLinear = null;
            foreach (Neuron neuron in brain.Neurons)
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
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input = InputNum.GREEN_BIBITE;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            SumNum sumNum = new SumNum(input, input);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { input.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
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
            NeuralBackgroundBrainBuilder.ClearBrain();
            InputNum input123 = InputNum.GREEN_BIBITE;
            InputNum input4 = InputNum.RED_BIBITE;
            Neuron output = Outputs.PHERE_OUT_1;

            // Act
            SumNum sumNum = (SumNum)(input123 + input123 + input123 + input4);
            ValueConnectionTester.ConnectValueTo(sumNum, output);

            // Assert

            Brain brain = NeuralBackgroundBrainBuilder.GetBrain();

            // there are the input and output neurons
            Neuron[] ioNeurons = new Neuron[] { input123.GetInputNeuron(), input4.GetInputNeuron(), output };
            foreach (Neuron neuron in ioNeurons)
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
