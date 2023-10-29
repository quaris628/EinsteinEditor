using Bibyte.neural;
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
    public abstract class Value
    {
        //protected internal abstract void ConnectTo(IEnumerable<Neuron> outputs);
        
        protected internal abstract void ConnectTo(IEnumerable<ConnectToRequest> outputConns);

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

        protected static IEnumerable<ConnectToRequest> multiplyAllConnsBy(IEnumerable<ConnectToRequest> conns, float scalar)
        {
            LinkedList<ConnectToRequest> newConnectToRequests = new LinkedList<ConnectToRequest>();
            foreach (ConnectToRequest outputConn in conns)
            {
                newConnectToRequests.AddLast(new ConnectToRequest(
                    outputConn.Neuron, outputConn.SynapseStrength * scalar));
            }
            return newConnectToRequests;
        }

        protected static void connectAndHandleLargeScalars(Neuron inputNeuron,
            IEnumerable<ConnectToRequest> outputConns)
        {
            List<Neuron> x100Neurons = new List<Neuron>();
            foreach (ConnectToRequest outputConn in outputConns)
            {
                float absNetScalar = Math.Abs(outputConn.SynapseStrength);
                bool isNetScalarPositive = outputConn.SynapseStrength > 0;
                int x100NeuronIndex = -1;
                while (BibiteVersionConfig.SYNAPSE_STRENGTH_MAX < absNetScalar)
                {
                    x100NeuronIndex++;
                    if (x100Neurons.Count <= x100NeuronIndex)
                    {
                        Neuron baseLinear = x100Neurons.Count == 0
                            ? inputNeuron
                            : x100Neurons.ElementAt(x100NeuronIndex);
                        Neuron newLinear = NeuronFactory.CreateNeuron(NeuronType.Linear);
                        SynapseFactory.CreateSynapse(baseLinear, newLinear, BibiteVersionConfig.SYNAPSE_STRENGTH_MAX);
                        x100Neurons.Add(newLinear);
                    }
                    absNetScalar /= BibiteVersionConfig.SYNAPSE_STRENGTH_MAX;
                }
                if (x100NeuronIndex >= 0)
                {
                    SynapseFactory.CreateSynapse(x100Neurons.ElementAt(x100NeuronIndex), outputConn.Neuron,
                        isNetScalarPositive ? absNetScalar : -absNetScalar);
                }
                else
                {
                    SynapseFactory.CreateSynapse(inputNeuron, outputConn.Neuron,
                        isNetScalarPositive ? absNetScalar : -absNetScalar);
                }
            }
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
