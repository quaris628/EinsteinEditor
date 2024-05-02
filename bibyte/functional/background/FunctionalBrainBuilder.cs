using Bibyte.neural;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibyte.functional.background
{
    public class FunctionalBrainBuilder
    {
        public static JsonBrain Build(IFunctionalProgrammingBrain brain)
        {
            NeuralBackgroundBrainBuilder.ClearBrain();
            brain.InitializeReusedValues();
            brain.Accelerate().PlugIntoOutput(Outputs.ACCELERATE);
            brain.Rotate().PlugIntoOutput(Outputs.ROTATE);
            brain.Herding().PlugIntoOutput(Outputs.HERDING);
            brain.Want2Lay().PlugIntoOutput(Outputs.WANT_2_LAY);
            brain.Digestion().PlugIntoOutput(Outputs.DIGESTION);
            brain.Grab().PlugIntoOutput(Outputs.GRAB);
            brain.ClkReset().PlugIntoOutput(Outputs.CLK_RESET);
            brain.PhereOut1().PlugIntoOutput(Outputs.PHERE_OUT_1);
            brain.PhereOut2().PlugIntoOutput(Outputs.PHERE_OUT_2);
            brain.PhereOut3().PlugIntoOutput(Outputs.PHERE_OUT_3);
            brain.Want2Grow().PlugIntoOutput(Outputs.WANT_2_GROW);
            brain.Want2Heal().PlugIntoOutput(Outputs.WANT_2_HEAL);
            brain.Want2Attack().PlugIntoOutput(Outputs.WANT_2_ATTACK);
            brain.ImmuneSystem().PlugIntoOutput(Outputs.IMMUNE_SYSTEM);
            return NeuralBackgroundBrainBuilder.GetBrain();
        }
    }
}
