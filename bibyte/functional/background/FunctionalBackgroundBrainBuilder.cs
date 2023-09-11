using Bibyte.neural;
using Einstein;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional
{
    public class FunctionalBackgroundBrainBuilder
    {
        public static Brain Build(IFunctionalProgrammingBrain brain)
        {
            NeuralBackgroundBrainBuilder.ClearBrain();
            brain.Accelerate().AddOutput(Outputs.ACCELERATE);
            brain.Rotate().AddOutput(Outputs.ROTATE);
            brain.Herding().AddOutput(Outputs.HERDING);
            brain.Want2Lay().AddOutput(Outputs.WANT_2_LAY);
            brain.Digestion().AddOutput(Outputs.DIGESTION);
            brain.Grab().AddOutput(Outputs.GRAB);
            brain.ClkReset().AddOutput(Outputs.CLK_RESET);
            brain.PhereOut1().AddOutput(Outputs.PHERE_OUT_1);
            brain.PhereOut2().AddOutput(Outputs.PHERE_OUT_2);
            brain.PhereOut3().AddOutput(Outputs.PHERE_OUT_3);
            brain.Want2Grow().AddOutput(Outputs.WANT_2_GROW);
            brain.Want2Heal().AddOutput(Outputs.WANT_2_HEAL);
            brain.Want2Attack().AddOutput(Outputs.WANT_2_ATTACK);
            brain.ImmuneSystem().AddOutput(Outputs.IMMUNE_SYSTEM);
            return NeuralBackgroundBrainBuilder.GetBrain();
        }
    }
}
