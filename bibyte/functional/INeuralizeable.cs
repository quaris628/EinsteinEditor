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
        void AddSynapsesTo(Neuron output);
    }
}
