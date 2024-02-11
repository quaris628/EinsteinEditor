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
    public class BibiteVersion0_5 : BibiteVersion
    {
        internal static readonly BibiteVersion0_5 INSTANCE = new BibiteVersion0_5();

        private BibiteVersion0_5(): base(5)
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

        public override bool GetNeuronDiagramPositionFromRawJsonFields(RawJsonFields fields, ref int x, ref int y)
        {
            // fall back to inov if it's not in the description
            return GetNeuronDiagramPositionFromDescription(fields, ref x, ref y)
                || GetNeuronDiagramPositionFromInov(fields, ref x, ref y);
        }
        public override void SetNeuronDiagramPositionInRawJsonFields(RawJsonFields fields, int x, int y)
        {
            SetNeuronDiagramPositionInDesc(fields, x, y);
            // always set inov
            SetNeuronDiagramPositionInInov(fields, x, y);
        }

        // No changes between 0.4 and 0.5

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            // To 0.4
            // deep copy with no changes
            return new JsonBrain(brain, V0_4);
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.6.0a
            // Add 2 new IO neurons, and shift indexes to accomodate them

            BaseBrain brainOut = new JsonBrain(V0_6_0a);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertNeuronIndexTo0_6_0a(neuron.Index);
                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, neuron.Description, V0_6_0a);
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;

                // set inov for Input/output neurons to 1 + index
                if (jn.IsInput() || jn.IsOutput())
                {
                    jn.Inov = newIndex + 1;
                }

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertNeuronIndexTo0_6_0a(synapse.To.Index);
                int newFromIndex = ConvertNeuronIndexTo0_6_0a(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            // EggsStored
            brainOut.Add(new JsonNeuron(8, NeuronType.Input, V0_6_0a.DESCRIPTIONS[8], V0_6_0a));
            // EggProduction
            brainOut.Add(new JsonNeuron(37, NeuronType.TanH, V0_6_0a.DESCRIPTIONS[37], V0_6_0a));

            return brainOut;
        }

        private int ConvertNeuronIndexTo0_6_0a(int index0_5)
        {
            if (0 <= index0_5 && index0_5 <= 7)
            {
                return index0_5;
            }
            else if (8 <= index0_5 && index0_5 <= 35)
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
