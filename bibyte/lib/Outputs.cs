using Einstein;
using Einstein.model.json;
using System;

namespace Bibyte
{
    public class Outputs
    {
        public static Neuron ACCELERATE       = ConstructOutputNeuron(33);
        public static Neuron ROTATE           = ConstructOutputNeuron(34);
        public static Neuron HERDING          = ConstructOutputNeuron(35);
        public static Neuron WANT_2_LAY       = ConstructOutputNeuron(36);
        public static Neuron WANT_2_EAT       = ConstructOutputNeuron(37);
        public static Neuron DIGESTION        = ConstructOutputNeuron(38);
        public static Neuron GRAB             = ConstructOutputNeuron(39);
        public static Neuron CLK_RESET        = ConstructOutputNeuron(40);
        public static Neuron PHERE_OUT_1      = ConstructOutputNeuron(41);
        public static Neuron PHERE_OUT_2      = ConstructOutputNeuron(42);
        public static Neuron PHERE_OUT_3      = ConstructOutputNeuron(43);
        public static Neuron WANT_2_GROW      = ConstructOutputNeuron(44);
        public static Neuron WANT_2_HEAL      = ConstructOutputNeuron(45);
        public static Neuron WANT_2_ATTACK    = ConstructOutputNeuron(46);
        public static Neuron IMMUNE_SYSTEM    = ConstructOutputNeuron(47);
      
        private static Neuron ConstructOutputNeuron(int index)
        {
            if (index < BibiteVersionConfig.OUTPUT_NODES_INDEX_MIN
            || BibiteVersionConfig.OUTPUT_NODES_INDEX_MAX < index)
            {
                throw new ArgumentException("bad index");
            }
            return new Neuron(index,
                    BibiteVersionConfig.GetOutputNeuronType(index),
                    BibiteVersionConfig.DESCRIPTIONS[index]);
        }
    }
}
