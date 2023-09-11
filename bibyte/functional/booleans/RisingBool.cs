﻿using Bibyte.functional.booleans;
using Bibyte.functional.values;
using Bibyte.neural;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional.booleans
{
    public class RisingBool : Bool
    {
        private Bool input;

        public RisingBool(Bool input)
        {
            this.input = input;
        }

        public override void ConnectTo(Neuron output, float outputSynapseStrengthOverride)
        {
            if (input is ConstBool)
            {
                new ConstBool(false).AddOutput(output);
                return;
            }
            // if you change the number of synapses between the input and output,
            // then update StoredValue accordingly
            Neuron diff = NeuronFactory.CreateNeuron(NeuronType.Differential, "RisingBool");
            Neuron latch = NeuronFactory.CreateNeuron(NeuronType.Latch, "RisingBool");
            input.ConnectTo(diff, 100f);
            SynapseFactory.CreateSynapse(diff, latch, 100f);
            (new ConstVal(-1)).AddOutputSynapse(latch);
            SynapseFactory.CreateSynapse(latch, output, outputSynapseStrengthOverride);
        }
    }
}
