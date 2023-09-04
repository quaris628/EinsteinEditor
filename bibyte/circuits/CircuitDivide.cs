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
    /// Approximates numerator / denominator.
    /// </summary>
    public class CircuitDivide : Circuit1OverX
    {
        Neuron numerator;
        
        public CircuitDivide(Neuron numerator, Neuron denominator)
            : base(denominator)
        {
            this.numerator = numerator;

            BackgroundBrainBuilder.AddToBrain(new Synapse(numerator, GetQuotient(), 1));
        }

        public Neuron GetNumerator() { return numerator; }
    }
}
