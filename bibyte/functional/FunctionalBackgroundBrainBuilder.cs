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
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Accelerate().GetSynapsesTo(Outputs.ACCELERATE));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Rotate().GetSynapsesTo(Outputs.ROTATE));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Herding().GetSynapsesTo(Outputs.HERDING));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Want2Lay().GetSynapsesTo(Outputs.WANT_2_LAY));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Digestion().GetSynapsesTo(Outputs.DIGESTION));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Grab().GetSynapsesTo(Outputs.GRAB));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.ClkReset().GetSynapsesTo(Outputs.CLK_RESET));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.PhereOut1().GetSynapsesTo(Outputs.PHERE_OUT_1));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.PhereOut2().GetSynapsesTo(Outputs.PHERE_OUT_2));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.PhereOut3().GetSynapsesTo(Outputs.PHERE_OUT_3));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Want2Grow().GetSynapsesTo(Outputs.WANT_2_GROW));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Want2Heal().GetSynapsesTo(Outputs.WANT_2_HEAL));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.Want2Attack().GetSynapsesTo(Outputs.WANT_2_ATTACK));
            NeuralBackgroundBrainBuilder.AddToBrain(brain.ImmuneSystem().GetSynapsesTo(Outputs.IMMUNE_SYSTEM));
            return NeuralBackgroundBrainBuilder.GetBrain();
        }
    }
}
