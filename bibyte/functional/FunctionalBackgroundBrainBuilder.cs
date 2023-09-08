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
            brain.Accelerate().AddSynapsesTo(Outputs.ACCELERATE);
            brain.Rotate().AddSynapsesTo(Outputs.ROTATE);
            brain.Herding().AddSynapsesTo(Outputs.HERDING);
            brain.Want2Lay().AddSynapsesTo(Outputs.WANT_2_LAY);
            brain.Digestion().AddSynapsesTo(Outputs.DIGESTION);
            brain.Grab().AddSynapsesTo(Outputs.GRAB);
            brain.ClkReset().AddSynapsesTo(Outputs.CLK_RESET);
            brain.PhereOut1().AddSynapsesTo(Outputs.PHERE_OUT_1);
            brain.PhereOut2().AddSynapsesTo(Outputs.PHERE_OUT_2);
            brain.PhereOut3().AddSynapsesTo(Outputs.PHERE_OUT_3);
            brain.Want2Grow().AddSynapsesTo(Outputs.WANT_2_GROW);
            brain.Want2Heal().AddSynapsesTo(Outputs.WANT_2_HEAL);
            brain.Want2Attack().AddSynapsesTo(Outputs.WANT_2_ATTACK);
            brain.ImmuneSystem().AddSynapsesTo(Outputs.IMMUNE_SYSTEM);
            return NeuralBackgroundBrainBuilder.GetBrain();
        }
    }
}
