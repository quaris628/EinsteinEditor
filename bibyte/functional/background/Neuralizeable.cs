using Einstein;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional.background
{
    public abstract class Neuralizeable
    {
        private LinkedList<Neuron> outputs;
        public void AddOutput(Neuron output)
        {
            outputs.AddFirst(output);
        }
        public void Build()
        {
            ConnectTo(outputs);
        }
        // this is only public to avoid compiler error CS1540 in BoolToValVal
        // if not for that, this would be protected
        public abstract void ConnectTo(IEnumerable<Neuron> outputs);

        protected static bool containsMults(IEnumerable<Neuron> neurons)
        {
            foreach (Neuron neuron in neurons)
            {
                if (neuron.Type == NeuronType.Mult)
                {
                    return true;
                }
            }
            return false;
        }
        protected static bool containsNonMults(IEnumerable<Neuron> neurons)
        {
            foreach (Neuron neuron in neurons)
            {
                if (neuron.Type != NeuronType.Mult)
                {
                    return true;
                }
            }
            return false;
        }

        protected static void validateFloat(float val)
        {
            if (val < BibiteVersionConfig.SYNAPSE_STRENGTH_MIN
            || BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < val)
            {
                throw new ArgumentException("bad value, must be between -100 and 100");
            }
        }
    }
}
