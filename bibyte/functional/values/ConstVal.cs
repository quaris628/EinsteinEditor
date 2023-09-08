﻿using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.values
{
    /// <summary>
    /// A constant value that does not change.
    /// </summary>
    public class ConstVal : Value
    {
        private float value;
        public ConstVal(float value)
        {
            validateFloat(value);
            this.value = value;
        }

        public float GetValue() { return value; }

        public override void AddSynapsesTo(Neuron output)
        {
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, output, value);
        }
    }
}
