﻿using Bibyte.neural;
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

        /// <summary>
        /// This is only an approximation of division and breaks when the denominator is near zero.
        /// The error is less than 1% when the denominator is farther than 0.1 from zero,
        /// and the error is less than 10% when the denominator is farther than 0.03 from zero.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public InverseVal(Value val)
        {
            this.val = val;
        }

        public override void ConnectTo(IEnumerable<Neuron> outputs)
        {
            // could get more and more precise with more parallel nodes
            // (optional, could do later)

            Neuron linear = NeuronFactory.CreateNeuron(NeuronType.Linear, "Inverse");
            Neuron gauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian, "Inverse");
            val.AddOutput(linear);
            SynapseFactory.CreateSynapse(linear, gauss, 100f);

            if (containsNonMults(outputs))
            {
                Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "Inverse");
                SynapseFactory.CreateSynapse(linear, mult, 100f);
                SynapseFactory.CreateSynapse(gauss, mult, 100f);
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(mult, output, 1f);
                }
            }
            else
            {
                foreach (Neuron output in outputs)
                {
                    SynapseFactory.CreateSynapse(linear, output, 100f);
                    SynapseFactory.CreateSynapse(gauss, output, 100f);
                }
            }
        }
    }
}
