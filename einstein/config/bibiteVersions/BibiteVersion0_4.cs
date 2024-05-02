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
    public class BibiteVersion0_4 : BibiteVersion
    {
        internal static readonly BibiteVersion0_4 INSTANCE = new BibiteVersion0_4();

        private BibiteVersion0_4(): base(4)
        {
            VERSION_NAME = "0.4";

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

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            throw new NoSuchVersionException("There is no supported version lower than " + VERSION_NAME);
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.5
            // deep copy with no changes
            return new JsonBrain(brain, V0_5);
        }
    }
}
