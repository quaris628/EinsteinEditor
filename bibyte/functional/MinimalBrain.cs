﻿using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.functional.values;
using Bibyte.functional.memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    public class MinimalBrain : IFunctionalProgrammingBrain
    {
        public Value Accelerate()
        {
            return Func.Gauss(InputVal.PLANT_ANGLE * 4);
        }
        public Value Rotate()
        {
            return InputVal.PLANT_ANGLE * 4;
        }
        public Value Herding()
        {
            return new ConstVal(0);
        }
        public Value Want2Lay()
        {
            return new ConstVal(0);
        }
        public Value Digestion()
        {
            return new IfVal(InputVal.ENERGY_RATIO < 0.9f, new ConstVal(2), new ConstVal(-2));
        }
        public Value Grab()
        {
            return InputVal.FULLNESS;
        }
        public Value ClkReset()
        {
            return new ConstVal(0);
        }
        public Value PhereOut1()
        {
            return new ConstVal(0);
        }
        public Value PhereOut2()
        {
            return new ConstVal(0);
        }
        public Value PhereOut3()
        {
            return new ConstVal(0);
        }
        public Value Want2Grow()
        {
            return new ConstVal(0);
        }
        public Value Want2Heal()
        {
            return new ConstVal(0);
        }
        public Value Want2Attack()
        {
            return new ConstVal(0);
        }
        public Value ImmuneSystem()
        {
            return new ConstVal(0);
        }
    }
}
