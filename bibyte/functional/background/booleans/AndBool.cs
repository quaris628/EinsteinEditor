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
    /// <summary>
    /// A boolean that returns true if both input booleans are true.
    /// </summary>
    public class AndBool : Bool
    {
        private Bool left;
        private Bool right;

        public AndBool(Bool left, Bool right)
        {
            this.left = left;
            this.right = right;
        }

        protected internal override void ConnectTo(IEnumerable<ConnectToRequest> outputConns)
        {
            if (containsNonMults(outputConns))
            {
                Neuron mult = NeuronFactory.CreateNeuron(NeuronType.Mult, "And");
                left.ConnectTo(new[] { new ConnectToRequest(mult, 1f) });
                right.ConnectTo(new[] { new ConnectToRequest(mult, 1f) });
                foreach (ConnectToRequest outputConn in outputConns)
                {
                    SynapseFactory.CreateSynapse(mult, outputConn.Neuron, outputConn.SynapseStrength);
                }
            }
            else
            {
                foreach (ConnectToRequest outputConn in outputConns)
                {
                    left.ConnectTo(outputConns);
                    right.ConnectTo(outputConns);
                }
            }
        }
    }
}