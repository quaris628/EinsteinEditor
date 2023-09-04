using Einstein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.lib
{
    public class Strength
    {
        public float Value { get; }
        public Strength(float value)
        {
            if (value < BibiteVersionConfig.SYNAPSE_STRENGTH_MIN
            || BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < value)
            {
                throw new ArgumentException("Invalid synapse strength: " + value);
            }
            Value = value;
        }
    }
}
