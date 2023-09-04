using Bibyte.lib;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.circuits
{
    /// <summary>
    /// Approximates 1/Input.
    /// </summary>
    public class Circuit1OverX : Circuit
    {
        public static float DEFAULT_K = 100f;

        private Neuron denominator;
        private Neuron outputMult;
        
        public Circuit1OverX(Neuron input)
        {
            Neuron gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "DivGauss");
            outputMult = NeuronFactory.CreateNeuron(NeuronType.Mult);
            synapses = new Synapse[] {
                new Synapse(input, gauss, DEFAULT_K),
                new Synapse(gauss, outputMult, DEFAULT_K),
                new Synapse(input, outputMult, DEFAULT_K),
            };
        }

        public Neuron GetDenominator() { return denominator; }
        public Neuron GetQuotient() { return outputMult; }
    }
}
