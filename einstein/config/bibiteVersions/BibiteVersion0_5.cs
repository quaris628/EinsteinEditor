using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config.bibiteVersions
{
    public class BibiteVersion0_5 : BibiteVersion
    {
        internal static readonly BibiteVersion0_5 INSTANCE = new BibiteVersion0_5();

        private BibiteVersion0_5(): base(0)
        {
            VERSION_NAME = "0.5";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 32;
            OUTPUT_NODES_INDEX_MIN = 33;
            OUTPUT_NODES_INDEX_MAX = 47;
            HIDDEN_NODES_INDEX_MIN = 48;
            HIDDEN_NODES_INDEX_MAX = int.MaxValue;

            DESCRIPTIONS = new string[] {
                // ----- Inputs -----
                "Constant",
                "EnergyRatio",
                "Maturity",
                "LifeRatio",
                "Fullness",
                "Speed",
                "IsGrabbing",
                "AttackedDamage",
                "BibiteCloseness",
                "BibiteAngle",
                "NBibites",
                "PlantCloseness",
                "PlantAngle",
                "NPlants",
                "MeatCloseness",
                "MeatAngle",
                "NMeats",
                "RedBibite",
                "GreenBibite",
                "BlueBibite",
                "Tic",
                "Minute",
                "TimeAlive",
                "PheroSense1",
                "PheroSense2",
                "PheroSense3",
                "Phero1Angle",
                "Phero2Angle",
                "Phero3Angle",
                "Phero1Heading",
                "Phero2Heading",
                "Phero3Heading",
                "InfectionRate",
                // ----- Outputs -----
                "Accelerate",
                "Rotate",
                "Herding",
                "Want2Lay",
                "Want2Eat",
                "Digestion",
                "Grab",
                "ClkReset",
                "PhereOut1",
                "PhereOut2",
                "PhereOut3",
                "Want2Grow",
                "Want2Heal",
                "Want2Attack",
                "ImmuneSystem",
            };

            outputTypes = new NeuronType[]
            {
                NeuronType.TanH,
                NeuronType.TanH,
                NeuronType.TanH,
                NeuronType.Sigmoid,
                NeuronType.Sigmoid,
                NeuronType.Sigmoid,
                NeuronType.TanH,
                NeuronType.Sigmoid,
                NeuronType.ReLu,
                NeuronType.ReLu,
                NeuronType.ReLu,
                NeuronType.Sigmoid,
                NeuronType.Sigmoid,
                NeuronType.Sigmoid,
                NeuronType.TanH,
            };
        }

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            throw new NoSuchVersionException("There is no version lower than " + VERSION_NAME);
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.6
            // Add 2 new IO neurons, and shift indexes to accomodate them

            BaseBrain brainOut = new JsonBrain(V0_6);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertNeuronIndexTo0_6(neuron.Index);

                brainOut.Add(new JsonNeuron(newIndex, neuron.Type, neuron.Description, V0_6));
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertNeuronIndexTo0_6(synapse.To.Index);
                int newFromIndex = ConvertNeuronIndexTo0_6(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newTo, (JsonNeuron)newFrom, synapse.Strength));
            }

            // EggsStored
            brainOut.Add(new JsonNeuron(8, NeuronType.Input, V0_6.DESCRIPTIONS[8], V0_6));
            // EggProduction
            brainOut.Add(new JsonNeuron(37, NeuronType.TanH, V0_6.DESCRIPTIONS[37], V0_6));

            return brainOut;
        }

        private int ConvertNeuronIndexTo0_6(int index0_5)
        {
            if (0 <= index0_5 && index0_5 <= 8)
            {
                return index0_5;
            }
            else if (9 <= index0_5 && index0_5 <= 35)
            {
                return index0_5 + 1;
            }
            else if (36 <= index0_5)
            {
                return index0_5 + 2;
            }
            else
            {
                throw new ArgumentException("index cannot be negative");
            }
        }
    }
}
