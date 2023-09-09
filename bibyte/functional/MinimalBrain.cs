using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.functional.values;
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
            return InputVal.PHERO_SENSE_1 + InputVal.PHERO_SENSE_2;
        }
        public Value Rotate()
        {
            return (new BoolToValVal(new ValEqualsValBool(InputVal.GREEN_BIBITE, InputVal.RED_BIBITE))) * 1.5f;
        }
        public Value Herding()
        {
            return -InputVal.PLANT_CLOSENESS;
        }
        public Value Want2Lay()
        {
            return Func.Cos(InputVal.BIBITE_ANGLE);
        }
        public Value Digestion()
        {
            return new ConstVal(0);
        }
        public Value Grab()
        {
            return new ConstVal(0);
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
