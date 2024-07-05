using bibyte.functional.brains;
using Bibyte.neural;
using Einstein.config.bibiteVersions;
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
        public static JsonBrain Build(IFunctionalProgrammingBrain brain, out BibiteVersion bibiteVersion)
        {
            if (brain is IFunctionalProgrammingBrain0_5)
            {
                return Build(brain as IFunctionalProgrammingBrain0_5, out bibiteVersion);
            }
            else if (brain is IFunctionalProgrammingBrain0_6_0a)
            {
                return Build(brain as IFunctionalProgrammingBrain0_6_0a, out bibiteVersion);
            }
            else
            {
                throw new ArgumentException("Bibyte hasn't been updated (yet) to support this brain version.");
            }
        }
        
        public static JsonBrain Build(IFunctionalProgrammingBrain0_5 brain, out BibiteVersion bibiteVersion)
        {
            bibiteVersion = BibiteVersion.V0_5;
            NeuronFactory.InitializeBibiteVersion(bibiteVersion);
            NeuralBackgroundBrainBuilder.ClearBrain(bibiteVersion);
            brain.InitializeReusedValues();
            brain.Accelerate().PlugIntoOutput(Outputs0_5.ACCELERATE);
            brain.Rotate().PlugIntoOutput(Outputs0_5.ROTATE);
            brain.Herding().PlugIntoOutput(Outputs0_5.HERDING);
            brain.Want2Lay().PlugIntoOutput(Outputs0_5.WANT_2_LAY);
            brain.Digestion().PlugIntoOutput(Outputs0_5.DIGESTION);
            brain.Grab().PlugIntoOutput(Outputs0_5.GRAB);
            brain.ClkReset().PlugIntoOutput(Outputs0_5.CLK_RESET);
            brain.PhereOut1().PlugIntoOutput(Outputs0_5.PHERE_OUT_1);
            brain.PhereOut2().PlugIntoOutput(Outputs0_5.PHERE_OUT_2);
            brain.PhereOut3().PlugIntoOutput(Outputs0_5.PHERE_OUT_3);
            brain.Want2Grow().PlugIntoOutput(Outputs0_5.WANT_2_GROW);
            brain.Want2Heal().PlugIntoOutput(Outputs0_5.WANT_2_HEAL);
            brain.Want2Attack().PlugIntoOutput(Outputs0_5.WANT_2_ATTACK);
            brain.ImmuneSystem().PlugIntoOutput(Outputs0_5.IMMUNE_SYSTEM);
            return NeuralBackgroundBrainBuilder.GetBrain();
        }

        public static JsonBrain Build(IFunctionalProgrammingBrain0_6_0a brain, out BibiteVersion bibiteVersion)
        {
            bibiteVersion = BibiteVersion.V0_6_0a;
            NeuronFactory.InitializeBibiteVersion(bibiteVersion);
            NeuralBackgroundBrainBuilder.ClearBrain(bibiteVersion);
            brain.InitializeReusedValues();
            brain.Accelerate().PlugIntoOutput(Outputs0_6_0a.ACCELERATE);
            brain.Rotate().PlugIntoOutput(Outputs0_6_0a.ROTATE);
            brain.Herding().PlugIntoOutput(Outputs0_6_0a.HERDING);
            brain.EggProduction().PlugIntoOutput(Outputs0_6_0a.EGG_PRODUCTION);
            brain.Want2Lay().PlugIntoOutput(Outputs0_6_0a.WANT_2_LAY);
            brain.Digestion().PlugIntoOutput(Outputs0_6_0a.DIGESTION);
            brain.Grab().PlugIntoOutput(Outputs0_6_0a.GRAB);
            brain.ClkReset().PlugIntoOutput(Outputs0_6_0a.CLK_RESET);
            brain.PhereOut1().PlugIntoOutput(Outputs0_6_0a.PHERE_OUT_1);
            brain.PhereOut2().PlugIntoOutput(Outputs0_6_0a.PHERE_OUT_2);
            brain.PhereOut3().PlugIntoOutput(Outputs0_6_0a.PHERE_OUT_3);
            brain.Want2Grow().PlugIntoOutput(Outputs0_6_0a.WANT_2_GROW);
            brain.Want2Heal().PlugIntoOutput(Outputs0_6_0a.WANT_2_HEAL);
            brain.Want2Attack().PlugIntoOutput(Outputs0_6_0a.WANT_2_ATTACK);
            brain.ImmuneSystem().PlugIntoOutput(Outputs0_6_0a.IMMUNE_SYSTEM);
            return NeuralBackgroundBrainBuilder.GetBrain();
        }
    }
}
