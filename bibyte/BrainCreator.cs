using Einstein;
using Einstein.model;
using Einstein.model.json;

namespace Bibyte
{
   public static partial class BrainCreator
   {
      public static string BB8_FILE_TO_SAVE_TO =
         "C:/<YOUR BIBITES DIRECTORY HERE>/Bibites/<BIBITE NAME>.bb8";
      public static Brain GenerateBrain()
      {
         Synapse moveForward = new Synapse(Inputs.CONSTANT, Outputs.ACCELERATE, 1);
         Synapse turnToPlant = new Synapse(Inputs.PLANT_ANGLE, Outputs.ROTATE, 1);

         Neuron hiddenGauss = createNeuron(NeuronType.Gaussian);
         Neuron hiddenSine = createNeuron(NeuronType.Sine, "Sine");

         Synapse[] otherSynapses = {
            new Synapse(Inputs.PLANT_CLOSENESS, hiddenGauss, -2),
            new Synapse(hiddenGauss, hiddenSine, 8.675309f),
            new Synapse(hiddenGauss, Outputs.PHERE_OUT_3, 100),
         };

         // for larger brains you'll probably want to organize synapses in sub-collections
         return buildBrainFrom(otherSynapses,
            new Synapse[] { moveForward, turnToPlant });

         // but this method works better for smaller brains
         //return buildBrainFrom(moveForward, turnToPlant);
      }
   }
}
