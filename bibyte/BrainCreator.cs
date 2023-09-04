using Einstein;
using Einstein.model;
using Einstein.model.json;

namespace Bibyte
{
   public static partial class BrainCreator
   {
      public static string BB8_FILE_TO_SAVE_TO =
         "C:/<YOUR BIBITES DIRECTORY HERE>/Bibites/<BIBITE NAME>.bb8";
      public static JsonBrain GenerateBrain()
      {
         JsonSynapse moveForward = new JsonSynapse(Inputs.CONSTANT, Outputs.ACCELERATE, 1);
         JsonSynapse turnToPlant = new JsonSynapse(Inputs.PLANT_ANGLE, Outputs.ROTATE, 1);

         JsonNeuron hiddenGauss = createNeuron(NeuronType.Gaussian);
         JsonNeuron hiddenSine = createNeuron(NeuronType.Sine, "Sine");

         JsonSynapse[] otherSynapses = {
            new JsonSynapse(Inputs.PLANT_CLOSENESS, hiddenGauss, -2),
            new JsonSynapse(hiddenGauss, hiddenSine, 8.675309f),
            new JsonSynapse(hiddenGauss, Outputs.PHERE_OUT_3, 100),
         };

         // for larger brains you'll probably want to organize synapses in sub-collections
         return buildBrainFrom(otherSynapses,
            new JsonSynapse[] { moveForward, turnToPlant });

         // but this method works better for smaller brains
         //return buildBrainFrom(moveForward, turnToPlant);
      }
   }
}
