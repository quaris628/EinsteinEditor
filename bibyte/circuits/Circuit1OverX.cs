using Bibyte.neural;
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
    /// Approximates 1/denominator.
    /// </summary>
    public class Circuit1OverX
    {
        public static float DEFAULT_K = 100f;

        private Neuron denominator;
        private Neuron outputMult;
        
        public Circuit1OverX(Neuron denominator)
        {
            this.denominator = denominator;
            Neuron gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "DivGauss");
            outputMult = NeuronFactory.CreateNeuron(NeuronType.Mult);

            BackgroundBrainBuilder.AddToBrain(new Synapse[] {
                new Synapse(denominator, gauss, DEFAULT_K),
                new Synapse(gauss, outputMult, DEFAULT_K),
                new Synapse(denominator, outputMult, DEFAULT_K),
            });
        }

        public Neuron GetDenominator() { return denominator; }
        public Neuron GetQuotient() { return outputMult; }
    }
}
