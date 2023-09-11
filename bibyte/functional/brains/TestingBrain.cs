using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.functional.values;
using Bibyte.functional.memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bibyte.functional.booleans;

namespace Bibyte.functional
{
    public class TestingBrain : IFunctionalProgrammingBrain
    {
        public Value Accelerate()
        {
            return InputVal.GREEN_BIBITE + InputVal.GREEN_BIBITE + InputVal.GREEN_BIBITE;
        }
        public Value Rotate()
        {
            return new BoolToValVal(new ToggledBool(new RisingBool(InputVal.PLANT_ANGLE == InputVal.BIBITE_ANGLE), false));
        }
        public Value Herding()
        {
            return 0;
        }
        public Value Want2Lay()
        {
            return 0;
        }
        public Value Digestion()
        {
            return 0;
        }
        public Value Grab()
        {
            return 0;
        }
        public Value ClkReset()
        {
            return 0;
        }
        public Value PhereOut1()
        {
            return 0;
        }
        public Value PhereOut2()
        {
            return 0;
        }
        public Value PhereOut3()
        {
            return 0;
        }
        public Value Want2Grow()
        {
            return 0;
        }
        public Value Want2Heal()
        {
            return 0;
        }
        public Value Want2Attack()
        {
            return 0;
        }
        public Value ImmuneSystem()
        {
            return 0;
        }
    }
}
