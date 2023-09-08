using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.booleans
{
    public class ValLessThanEqValBool : Bool
    {
        public static float ERR_AFTER_LESS_THAN_OR_EQUAL = -0.00000400053346136992f;

        private Value left;
        private Value right;

        /// <summary>
        /// This is only an approximation of a less-than-or-equal-to.
        /// Once the left is less than or equal to the right, this will return true until
        /// the left is more than about 4x10^-6 i.e. 0.000004 greater than the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ValLessThanEqValBool(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }

        public override void AddSynapsesTo(Neuron output)
        {
            Neuron sigmoid = NeuronFactory.CreateNeuron(NeuronType.Sigmoid, "ValLessThanEqVal");
            (left * -100f).AddSynapsesTo(sigmoid);
            (right * 100f).AddSynapsesTo(sigmoid);

            Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValLessThanEqVal");
            SynapseFactory.CreateSynapse(sigmoid, latch, 100);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -49f);

            SynapseFactory.CreateSynapse(latch, output, 1);
        }
    }
}
