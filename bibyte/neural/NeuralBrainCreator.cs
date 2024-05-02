using Bibyte.circuits;
using Einstein;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;

namespace Bibyte.neural
{
    public static partial class NeuralBrainCreator
    {
        // Write your code here to create the brain.
        // Use SynapseFactory, NeuronFactory, and circuit classes.
        public static void CreateBrain()
        {
            BibiteVersion bibiteVersion = BibiteVersion.V0_5;
            NeuronFactory.InitializeBibiteVersion(bibiteVersion);
            NeuralBackgroundBrainBuilder.ClearBrain(bibiteVersion);

            SynapseFactory.CreateSynapse(Inputs0_5.CONSTANT, Outputs0_5.ACCELERATE, 1);
            SynapseFactory.CreateSynapse(Inputs0_5.PLANT_ANGLE, Outputs0_5.ROTATE, 1);

            CircuitDivide div = new CircuitDivide(Inputs0_5.PHERO_SENSE_1, Inputs0_5.PHERO_SENSE_2);
            SynapseFactory.CreateSynapse(div.GetQuotient(), Outputs0_5.HERDING, -1);

            JsonNeuron hiddenGauss = NeuronFactory.CreateNeuron(NeuronType.Gaussian);
            JsonNeuron hiddenSine = NeuronFactory.CreateNeuron(NeuronType.Sine, "Sine");

            SynapseFactory.CreateSynapse(Inputs0_5.PLANT_CLOSENESS, hiddenGauss, -2);
            SynapseFactory.CreateSynapse(hiddenGauss, hiddenSine, 8.675309f);
            SynapseFactory.CreateSynapse(hiddenSine, Outputs0_5.PHERE_OUT_3, 100);
        }
    }
}
