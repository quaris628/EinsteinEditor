using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Einstein.model.json.JsonNeuron;

namespace Einstein.config.bibiteVersions
{
    public class BibiteVersion0_6_0a : BibiteVersion
    {
        internal static readonly BibiteVersion0_6_0a INSTANCE = new BibiteVersion0_6_0a();

        private BibiteVersion0_6_0a(): base(6)
        {
            VERSION_NAME = "0.6.0a";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 33;
            OUTPUT_NODES_INDEX_MIN = 34;
            OUTPUT_NODES_INDEX_MAX = 49;
            HIDDEN_NODES_INDEX_MIN = 50;
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
                "EggStored", // added in 0.6
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
                "EggProduction", // added in 0.6
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
                NeuronType.TanH, // added in 0.6
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

        public override bool GetNeuronDiagramPositionFromRawJsonFields(RawJsonFields fields, ref int x, ref int y)
        {
            // fall back to inov if it's not in the description
            return GetNeuronDiagramPositionFromDescription(fields, ref x, ref y)
                || GetNeuronDiagramPositionFromInov(fields, ref x, ref y);
        }
        public override void SetNeuronDiagramPositionInRawJsonFields(RawJsonFields fields, int x, int y)
        {
            SetNeuronDiagramPositionInDesc(fields, x, y);
            // set inov if and only if the neuron type is hidden
            if (new JsonNeuron(fields, INSTANCE).IsHidden())
            {
                SetNeuronDiagramPositionInInov(fields, x, y);
            }
        }

        // Changes from 0.5:
        // - New input neuron EggsStored added at new index 8, offsets 
        // - New output neuron EggProduction added at new index 37, type is TanH
        //
        // Internal side effects:
        // all indices at 8 to 35 (inclusive) are now 1 higher
        // all indices that were at 36 and above are now 2 higher

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            // To 0.5
            // Remove the 2 new IO neurons, and shift indexes to fill the holes

            BaseBrain brainOut = new JsonBrain(V0_5);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertNeuronIndexTo0_5(neuron.Index);

                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, neuron.Description, V0_5);
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertNeuronIndexTo0_5(synapse.To.Index);
                int newFromIndex = ConvertNeuronIndexTo0_5(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            return brainOut;
        }

        private int ConvertNeuronIndexTo0_5(int index0_6)
        {
            if (0 <= index0_6 && index0_6 <= 7)
            {
                return index0_6;
            }
            else if (8 == index0_6)
            {
                throw new CannotConvertException("This brain contains an EggsStored neuron, which does not exist in version " + V0_5.VERSION_NAME);
            }
            else if (9 <= index0_6 && index0_6 <= 36)
            {
                return index0_6 - 1;
            }
            else if (37 == index0_6)
            {
                throw new CannotConvertException("This brain contains an EggProduction neuron, which does not exist in version " + V0_5.VERSION_NAME);
            }
            else if (38 <= index0_6)
            {
                return index0_6 - 2;
            }
            else
            {
                throw new ArgumentException("index cannot be negative");
            }
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            throw new NoSuchVersionException("There is no supported version higher than " + VERSION_NAME);
        }
    }
}
