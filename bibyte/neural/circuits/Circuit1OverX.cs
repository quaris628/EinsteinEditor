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

        private JsonNeuron denominator;
        private JsonNeuron outputMult;
        
        public Circuit1OverX(JsonNeuron denominator)
        {
            this.denominator = denominator;
            JsonNeuron gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "DivGauss");
            outputMult = NeuronFactory.CreateNeuron(NeuronType.Mult);

            NeuralBackgroundBrainBuilder.AddToBrain(new JsonSynapse[] {
                new JsonSynapse(denominator, gauss, DEFAULT_K),
                new JsonSynapse(gauss, outputMult, DEFAULT_K),
                new JsonSynapse(denominator, outputMult, DEFAULT_K),
            });
        }

        public JsonNeuron GetDenominator() { return denominator; }
        public JsonNeuron GetQuotient() { return outputMult; }
    }
}
