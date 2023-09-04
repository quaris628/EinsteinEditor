using Bibyte.circuits;
using Bibyte.lib;
using Einstein;
using Einstein.model;
using Einstein.model.json;

namespace Bibyte
{
    public static partial class BrainCreator
    {
        public static string BB8_FILE_TO_SAVE_TO =
            "C:/Users/Quaris/AppData/LocalLow/The Bibites/The Bibites/Bibites/bibyteTest.bb8";
        public static Brain GenerateBrain()
        {
            Synapse moveForward = new Synapse(Inputs.CONSTANT, Outputs.ACCELERATE, 1);
            Synapse turnToPlant = new Synapse(Inputs.PLANT_ANGLE, Outputs.ROTATE, 1);

            CircuitDivide div = new CircuitDivide(Inputs.PHERO_SENSE_1, Inputs.PHERO_SENSE_2);
            Synapse randomDivide = new Synapse(div.GetQuotient(), Outputs.HERDING, -1);

            Neuron hiddenGauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian);
            Neuron hiddenSine = NeuronFactory.CreateNeuron(NeuronType.Sine, "Sine");

            Synapse[] otherSynapses = {
                new Synapse(Inputs.PLANT_CLOSENESS, hiddenGauss, -2),
                new Synapse(hiddenGauss, hiddenSine, 8.675309f),
                new Synapse(hiddenSine, Outputs.PHERE_OUT_3, 100),
            };

            // for larger brains you'll probably want to organize synapses in sub-collections
            return buildBrainFrom(otherSynapses,
                div.GetSynapses(),
                new Synapse[] { moveForward, turnToPlant, randomDivide });

            // but this method works better for smaller brains
            //return buildBrainFrom(moveForward, turnToPlant);
        }
    }
}
