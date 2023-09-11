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
    internal class ProductVal : Value
    {
        private List<Value> values;
        public ProductVal()
        {
            this.values = new List<Value>();
        }
        public ProductVal(List<Value> values)
        {
            this.values = values;
        }
        public void MultiplyBy(Value value)
        {
            values.Add(value);
        }
        public override void AddOutputSynapse(Neuron output)
        {
            if (output.Type == NeuronType.Mult)
            {
                foreach (Value value in values)
                {
                    value.AddOutput(output);
                }
            }
            else
            {
                Neuron multNeuron = NeuronFactory.CreateNeuron(NeuronType.Mult, "Product");
                foreach (Value value in values)
                {
                    value.AddOutput(multNeuron);
                }
                SynapseFactory.CreateSynapse(multNeuron, output, 1f);
            }
        }
    }
}