using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein
{
    // 0.5.0
    public struct BibiteVersionConfig
    {
        public const int INPUT_NODES_INDEX_MIN = 0;
        public const int INPUT_NODES_INDEX_MAX = 32;
        public const int OUTPUT_NODES_INDEX_MIN = 33;
        public const int OUTPUT_NODES_INDEX_MAX = 47;
        public const int HIDDEN_NODES_INDEX_MIN = 48;
        public const int HIDDEN_NODES_INDEX_MAX = int.MaxValue;

        public const float SYNAPSE_STRENGTH_MIN = -10f;
        public const float SYNAPSE_STRENGTH_MAX = 10f;

        public static readonly string[] DESCRIPTIONS = new string[] {
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

        private static readonly NeuronType[] OUTPUT_TYPES = new NeuronType[] {
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

        public static NeuronType GetOutputNeuronType(int index)
        {
            return OUTPUT_TYPES[index - OUTPUT_NODES_INDEX_MIN];
        }
    }
}
