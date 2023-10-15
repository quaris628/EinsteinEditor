﻿using bibyte.functional.background;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background.booleans
{
    public class ValLessThanValBool : Bool
    {
        public static float ERR_AFTER_LESS_THAN = -0.000004f;

        private Value left;
        private Value right;

        /// <summary>
        /// This is only an approximation of a less-than.
        /// The left value must be less than the right value by more than about 4x10^-6 or 0.000004
        /// before this returns true. Once that happens, the left value must be equal to
        /// or greater than the right value for this to return false.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ValLessThanValBool(Value left, Value right)
        {
            this.left = left;
            this.right = right;
        }
        
        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            Neuron sigmoid = NeuronFactory.CreateNeuron(NeuronType.Sigmoid, "ValLessThanVal");
            (left * -100f).ConnectTo(new[] { sigmoid });
            (right * 100f).ConnectTo(new[] { sigmoid });

            Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "ValLessThanVal");
            SynapseFactory.CreateSynapse(sigmoid, latch, 100);
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, latch, -49.99999f);

            foreach (ConnectToRequest outputConn in outputConns)
            {
                SynapseFactory.CreateSynapse(latch, outputConn.Neuron, outputConn.SynapseStrength);
            }
        }
    }
}