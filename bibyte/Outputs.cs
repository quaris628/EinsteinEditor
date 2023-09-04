using Einstein;
using Einstein.model.json;
using System;

namespace Bibyte
{
   public class Outputs
   {
      public static JsonNeuron ACCELERATE       = ConstructOutputNeuron(33);
      public static JsonNeuron ROTATE           = ConstructOutputNeuron(34);
      public static JsonNeuron HERDING          = ConstructOutputNeuron(35);
      public static JsonNeuron WANT_2_LAY       = ConstructOutputNeuron(36);
      public static JsonNeuron WANT_2_EAT       = ConstructOutputNeuron(37);
      public static JsonNeuron DIGESTION        = ConstructOutputNeuron(38);
      public static JsonNeuron GRAB             = ConstructOutputNeuron(39);
      public static JsonNeuron CLK_RESET        = ConstructOutputNeuron(40);
      public static JsonNeuron PHERE_OUT_1      = ConstructOutputNeuron(41);
      public static JsonNeuron PHERE_OUT_2      = ConstructOutputNeuron(42);
      public static JsonNeuron PHERE_OUT_3      = ConstructOutputNeuron(43);
      public static JsonNeuron WANT_2_GROW      = ConstructOutputNeuron(44);
      public static JsonNeuron WANT_2_HEAL      = ConstructOutputNeuron(45);
      public static JsonNeuron WANT_2_ATTACK    = ConstructOutputNeuron(46);
      public static JsonNeuron IMMUNE_SYSTEM    = ConstructOutputNeuron(47);
      
      private static JsonNeuron ConstructOutputNeuron(int index)
      {
         if (index < BibiteVersionConfig.OUTPUT_NODES_INDEX_MIN
            || BibiteVersionConfig.OUTPUT_NODES_INDEX_MAX < index)
         {
            throw new ArgumentException("bad index");
         }
         return new JsonNeuron(index,
                    BibiteVersionConfig.GetOutputNeuronType(index),
                    BibiteVersionConfig.DESCRIPTIONS[index]);
      }

   }
}
