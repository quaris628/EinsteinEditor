using Einstein.config.bibiteVersions;
using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.ui.editarea
{
    public class NeuronValueCalculator
    {
        private NeuronValueCalculator() { }

        /// <summary>
        /// Does one wave of neuron calculations.
        /// Does not update input neurons.
        /// </summary>
        /// <param name="brain"></param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        /// <returns>indexes of neurons with updated Values (not including LastInput)</returns>
        public static IEnumerable<int> Calc(BaseBrain brain, float deltaTime)
        {
            switch (brain.BibiteVersion.GetSynapseOrderCalcMethod())
            {
                case BibiteVersion.SynapseFiringCalcMethod.InOrder:
                    return CalcSynapsesFiringInOrder(brain, deltaTime);
                case BibiteVersion.SynapseFiringCalcMethod.Simultaneous:
                    return CalcSynapsesFiringSimultaneous(brain, deltaTime);
            }
            throw new ArgumentException(
                $"Unrecognized synapse firing calc method '{brain?.BibiteVersion?.GetSynapseOrderCalcMethod()}'");
        }

        private static IEnumerable<int> CalcSynapsesFiringSimultaneous(BaseBrain brain, float deltaTime)
        {
            Dictionary<BaseNeuron, float> neuronToNewValue = new Dictionary<BaseNeuron, float>();
            Dictionary<BaseNeuron, float> neuronToNewLastInput = new Dictionary<BaseNeuron, float>();
            HashSet<int> updatedNeuronsIndexes = new HashSet<int>();

            // DO NOT CONCURRENTLY MODIFY
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                if (neuron.Type == NeuronType.Input)
                {
                    continue;
                }
                float inputValue = CalcNeuronInput(neuron, brain);
                float outputValue = CalcNeuronOutput(neuron,
                    inputValue,
                    neuron.LastInput,
                    neuron.Value,
                    deltaTime,
                    neuron.Bias);
                outputValue = Math.Max(-100f, Math.Min(outputValue, 100f));

                neuronToNewLastInput[neuron] = inputValue;
                neuronToNewValue[neuron] = outputValue;
            }
            foreach (BaseNeuron neuron in neuronToNewValue.Keys)
            {
                neuron.LastInput = neuronToNewLastInput[neuron];
                float newValue = neuronToNewValue[neuron];
                if (neuron.Value != newValue)
                {
                    neuron.Value = newValue;
                    updatedNeuronsIndexes.Add(neuron.Index);
                }
            }

            return updatedNeuronsIndexes;
        }

        private static IEnumerable<int> CalcSynapsesFiringInOrder(BaseBrain brain, float deltaTime)
        {
            // Pseudocode:
            //   Iterate through the synapses in the order that they are listed in the json, and for each:
            //     If the synapse hasn't already fired:
            //       Fire all synapses that share this synapse's "To" neuron
            //
            // The brain is being concurrently modified during this iteration
            // (I don't think it should be, but that's how the sim does it, so...)

            HashSet<BaseSynapse> alreadyFiredSynapses = new HashSet<BaseSynapse>();
            HashSet<int> updatedNeuronsIndexes = new HashSet<int>();

            foreach (BaseSynapse synapse in brain.Synapses)
            {
                if (!alreadyFiredSynapses.Contains(synapse))
                {
                    BaseNeuron toNeuron = synapse.To;
                    if (synapse.To.Type == NeuronType.Input)
                    {
                        continue;
                    }

                    float inputValue = CalcNeuronInput(toNeuron, brain);
                    float outputValue = CalcNeuronOutput(toNeuron,
                        inputValue,
                        toNeuron.LastInput,
                        toNeuron.Value,
                        deltaTime,
                        toNeuron.Bias);
                    outputValue = Math.Max(-100f, Math.Min(outputValue, 100f));
                    if (toNeuron.Value != outputValue)
                    {
                        toNeuron.Value = outputValue;
                        updatedNeuronsIndexes.Add(toNeuron.Index);
                    }

                    foreach (BaseSynapse synapseFired in brain.GetSynapsesTo(synapse.To))
                    {
                        alreadyFiredSynapses.Add(synapseFired);
                    }
                }
            }

            return updatedNeuronsIndexes;
        }

        private static float CalcNeuronInput(BaseNeuron neuron, BaseBrain brain)
        {
            if (neuron.Type == NeuronType.Mult)
            {
                float product = 1f;
                foreach (BaseSynapse synapse in brain.GetSynapsesTo(neuron))
                {
                    product *= synapse.From.Value * synapse.Strength;
                }
                if (neuron.BibiteVersion.HasBiases())
                {
                    product *= neuron.Bias;
                }
                return product;
            }
            else
            {
                float sum = 0f;
                foreach (BaseSynapse synapse in brain.GetSynapsesTo(neuron))
                {
                    sum += synapse.From.Value * synapse.Strength;
                }
                sum += neuron.Bias;
                return sum;
            }
        }

        private static float CalcNeuronOutput(BaseNeuron neuron,
            float inputValue, float previousInput, float previousOutput,
            float dt, float bias)
        {
            switch (neuron.Type)
            {
                case NeuronType.Sigmoid:
                    return Sigmoid(inputValue);
                case NeuronType.Linear:
                    return Linear(inputValue);
                case NeuronType.TanH:
                    return TanH(inputValue);
                case NeuronType.Sine:
                    return Sine(inputValue);
                case NeuronType.ReLu:
                    return ReLu(inputValue);
                case NeuronType.Gaussian:
                    return Gaussian(inputValue);
                case NeuronType.Latch:
                    return Latch(inputValue, previousOutput);
                case NeuronType.Differential:
                    return Differential(inputValue, previousInput, dt);
                case NeuronType.Abs:
                    return Abs(inputValue);
                case NeuronType.Mult:
                    return Mult(inputValue);
                case NeuronType.Integrator:
                    return Integrator(inputValue, previousOutput, dt);
                case NeuronType.Inhibitory:
                    return Inhibitory(inputValue, previousInput, previousOutput, dt, bias);
                default:
                    throw new ArgumentException("Invalid Neuron Type, cannot calculate");
            }
        }

        private static float Sigmoid(float input)
        {
            return (float)(1 / (1 + Math.Exp(-input)));
        }
        private static float Linear(float input)
        {
            return input;
        }
        private static float TanH(float input)
        {
            return (float)Math.Tanh(input);
        }
        private static float Sine(float input)
        {
            return (float)Math.Sin(input); // radians
        }
        private static float ReLu(float input)
        {
            return Math.Max(0, input);
        }
        private static float Gaussian(float input)
        {
            return (float)(1 / (1 + input * input));
        }
        private static float Latch(float input, float previousOutput)
        {
            if (1f <= input)
            {
                return 1f;
            }
            else if (0f < input && input < 1f)
            {
                return previousOutput;
            }
            else // input <= 0
            {
                return 0f;
            }
        }
        private static float Differential(float input, float previousInput, float dt)
        {
            return (input - previousInput) / dt;
        }
        private static float Abs(float input)
        {
            return Math.Abs(input);
        }
        private static float Mult(float input)
        {
            return input;
        }
        private static float Integrator(float input, float previousOutput, float dt)
        {
            return previousOutput + input * dt;
        }
        private static float Inhibitory(float input, float previousInput, float previousOutput,
            float dt, float bias)
        {
            return (float) (input - previousInput + previousOutput * Math.Exp(-bias * dt));
        }
    }
}
