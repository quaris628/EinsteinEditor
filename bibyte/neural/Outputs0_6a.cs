using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model.json;
using System;

namespace Bibyte.neural
{
    public class Outputs0_6a
    {
        public static JsonNeuron ACCELERATE       = ConstructOutputNeuron(34);
        public static JsonNeuron ROTATE           = ConstructOutputNeuron(35);
        public static JsonNeuron HERDING          = ConstructOutputNeuron(36);
        public static JsonNeuron EGG_PRODUCTION   = ConstructOutputNeuron(37);
        public static JsonNeuron WANT_2_LAY       = ConstructOutputNeuron(38);
        public static JsonNeuron WANT_2_EAT       = ConstructOutputNeuron(39);
        public static JsonNeuron DIGESTION        = ConstructOutputNeuron(40);
        public static JsonNeuron GRAB             = ConstructOutputNeuron(41);
        public static JsonNeuron CLK_RESET        = ConstructOutputNeuron(42);
        public static JsonNeuron PHERE_OUT_1      = ConstructOutputNeuron(43);
        public static JsonNeuron PHERE_OUT_2      = ConstructOutputNeuron(44);
        public static JsonNeuron PHERE_OUT_3      = ConstructOutputNeuron(45);
        public static JsonNeuron WANT_2_GROW      = ConstructOutputNeuron(46);
        public static JsonNeuron WANT_2_HEAL      = ConstructOutputNeuron(47);
        public static JsonNeuron WANT_2_ATTACK    = ConstructOutputNeuron(48);
        public static JsonNeuron IMMUNE_SYSTEM    = ConstructOutputNeuron(49);
      
        private static JsonNeuron ConstructOutputNeuron(int index)
        {
            if (index < BibiteVersion.V0_6_0a.OUTPUT_NODES_INDEX_MIN
            || BibiteVersion.V0_6_0a.OUTPUT_NODES_INDEX_MAX < index)
            {
                throw new ArgumentException("bad index");
            }
            return new JsonNeuron(index,
                    BibiteVersion.V0_6_0a.GetOutputNeuronType(index),
                    BibiteVersion.V0_6_0a.DESCRIPTIONS[index],
                    BibiteVersion.V0_6_0a);
        }
    }
}
