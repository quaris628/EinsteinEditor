using bibyte.functional;
using Bibyte.functional.background;
using Bibyte.functional.background.booleans;
using Bibyte.functional.background.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    public class TestingBrain0_5 : IFunctionalProgrammingBrain0_5
    {
        private static Number risingPlantClose = null;
        public void InitializeReusedValues()
        {
            risingPlantClose = (Number)Func.Rising(InputNum.PLANT_CLOSENESS < 3);
        }
        public Number Accelerate()
        {
            return InputNum.GREEN_BIBITE + InputNum.GREEN_BIBITE + InputNum.GREEN_BIBITE;
        }
        public Number Rotate()
        {
            // Bad coding style, constructors feel awkward
            return new BoolToNumNum(new ToggledBool(new RisingBool(InputNum.PLANT_ANGLE == InputNum.BIBITE_ANGLE), false));
        }
        public Number Herding()
        {
            // Good coding style, functions and casts look better (imo)
            return (Number)Mem.ToggleOf(Func.Rising(InputNum.PLANT_ANGLE == InputNum.BIBITE_ANGLE), false);
        }
        public Number Want2Lay()
        {
            // Bad brain design to write duplicate expressions twice
            return (Number)Mem.ToggleOf(Func.Rising(InputNum.PLANT_ANGLE == InputNum.BIBITE_ANGLE), false);
        }
        
        public Number Digestion()
        {
            // Good brain design to initialize a commonly-reused expression once then reference it many times later
            return risingPlantClose;
        }
        public Number Grab()
        {
            return risingPlantClose;
        }
        public Number ClkReset()
        {
            return 2 * risingPlantClose;
        }
        public Number PhereOut1()
        {
            return 0;
        }
        public Number PhereOut2()
        {
            return 0;
        }
        public Number PhereOut3()
        {
            return 0;
        }
        public Number Want2Grow()
        {
            return 0;
        }
        public Number Want2Heal()
        {
            return 0;
        }
        public Number Want2Attack()
        {
            return 0;
        }
        public Number ImmuneSystem()
        {
            return 0;
        }
    }
}
