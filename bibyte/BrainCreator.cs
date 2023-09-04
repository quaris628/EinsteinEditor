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

        // Write your code here to create the brain.
        // Use SynapseFactory, NeuronFactory, and circuit classes.
        public static void CreateBrain()
        {
            SynapseFactory.CreateSynapse(Inputs.CONSTANT, Outputs.ACCELERATE, 1);
            SynapseFactory.CreateSynapse(Inputs.PLANT_ANGLE, Outputs.ROTATE, 1);

            CircuitDivide div = new CircuitDivide(Inputs.PHERO_SENSE_1, Inputs.PHERO_SENSE_2);
            SynapseFactory.CreateSynapse(div.GetQuotient(), Outputs.HERDING, -1);

            Neuron hiddenGauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian);
            Neuron hiddenSine = NeuronFactory.CreateNeuron(NeuronType.Sine, "Sine");

            SynapseFactory.CreateSynapse(Inputs.PLANT_CLOSENESS, hiddenGauss, -2);
            SynapseFactory.CreateSynapse(hiddenGauss, hiddenSine, 8.675309f);
            SynapseFactory.CreateSynapse(hiddenSine, Outputs.PHERE_OUT_3, 100);
        }
    }
}
