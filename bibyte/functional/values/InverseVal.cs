using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    public class InverseVal : Value
    {
        private Value val;
        public InverseVal(Value val)
        {
            this.val = val;
        }

        public override Synapse[] GetSynapsesTo(Neuron output)
        {
            // could get more and more precise with more parallel nodes
            // (optional, could do later)

            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "DivLinear");
            Neuron gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "DivGauss");
            Synapse[] synapsesToInput = val.GetSynapsesTo(linear);

            if (output.Type == NeuronType.Mult)
            {
                return synapsesToInput.Concat(new Synapse[] {
                    SynapseFactory.CreateSynapse(linear, gauss, 100f),
                    SynapseFactory.CreateSynapse(linear, output, 100f),
                    SynapseFactory.CreateSynapse(gauss, output, 100f),
                }).ToArray();
            }
            else
            {
                Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "DivMult");
                return synapsesToInput.Concat(new Synapse[] {
                    SynapseFactory.CreateSynapse(linear, gauss, 100f),
                    SynapseFactory.CreateSynapse(linear, mult, 100f),
                    SynapseFactory.CreateSynapse(gauss, mult, 100f),
                    SynapseFactory.CreateSynapse(mult, output, 1f),
                }).ToArray();
            }
        }
    }
}
