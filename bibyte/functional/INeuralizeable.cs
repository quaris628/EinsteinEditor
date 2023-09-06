using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    public interface INeuralizeable
    {
        // could change the array to an IEnumerable, ICollection, or even a List or something, idk...
        Synapse[] GetSynapsesTo(Neuron output);
    }
}
