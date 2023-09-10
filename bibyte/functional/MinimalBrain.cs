using Bibyte.functional;
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
            return InputVal.PHERO_SENSE_1 + InputVal.PHERO_SENSE_2;
        }
        public Value Rotate()
        {
            return (new IfVal(InputVal.PHERO_SENSE_1 == InputVal.RED_BIBITE, new ConstVal(5), new ConstVal(2))) * 2f;
        }
        public Value Herding()
        {
            return new StoredValue(InputVal.N_BIBITES > 2, InputVal.BIBITE_CLOSENESS);
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
            return new BoolToValVal(new StoredBool(new ConstBool(true), InputVal.RED_BIBITE == InputVal.BLUE_BIBITE));
        }
        public Value ClkReset()
        {
            return 2f ^ InputVal.SPEED;
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
