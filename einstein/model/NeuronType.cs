using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public enum NeuronType
    {
        Input = 0,
        Sigmoid = 1,
        Linear = 2,
        TanH = 3,
        Sine = 4,
        ReLu = 5,
        Gaussian = 6,
        Latch = 7,
        Differential = 8,
        Abs = 9,
        Mult = 10,
        Integrator = 11,
        Inhibitory = 12,
    }
}
