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
        protected internal abstract void ConnectTo(IEnumerable<Neuron> outputs);

        protected static IEnumerable<Neuron> neuronsOf(
            IEnumerable<ConnectToRequest> outputConns)
        {
            foreach (ConnectToRequest conn in outputConns)
            {
                yield return conn.Neuron;
            }
        }

        protected static bool containsMults(IEnumerable<ConnectToRequest> outputConns)
        {
            return containsMults(neuronsOf(outputConns));
        }
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

        protected static bool containsNonMults(IEnumerable<ConnectToRequest> outputConns)
        {
            return containsNonMults(neuronsOf(outputConns));
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
                throw new ArgumentException(val + " cannot be used as a synapse strength. "
                    + "Must be between -100 and 100.");
            }
        }
    }
}
