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
    /// Approximates numerator / denominator.
    /// </summary>
    public class CircuitDivide : Circuit1OverX
    {
        JsonNeuron numerator;
        
        public CircuitDivide(JsonNeuron numerator, JsonNeuron denominator)
            : base(denominator)
        {
            this.numerator = numerator;

            NeuralBackgroundBrainBuilder.AddToBrain(new JsonSynapse(numerator, GetQuotient(), 1));
        }

        public JsonNeuron GetNumerator() { return numerator; }
    }
}
