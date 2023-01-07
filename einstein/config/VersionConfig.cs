using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein
{
    public struct VersionConfig
    {
        public const int INPUT_NODES_INDEX_MIN = 0;
        public const int INPUT_NODES_INDEX_MAX = 3;//32;
        public const int OUTPUT_NODES_INDEX_MIN = 33;
        public const int OUTPUT_NODES_INDEX_MAX = 34;//47;
        public const int HIDDEN_NODES_INDEX_MIN = 48;
        public const int HIDDEN_NODES_INDEX_MAX = 50;//int.MaxValue;

        public static readonly string[] DESCRIPTIONS = new string[] {
            // ----- Inputs -----
            "Constant",
            "EnergyRatio",
            "Maturity",
            "LifeRatio",
            "Fullness",
            "Speed",
            "IsGrabbingObjects",
            "AttackedDamage",
            "BibiteConcentrationWeight",
            "BibiteAngle",
            "NVisibleBibites",
            "PelletConcentrationWeight",
            "PelletConcentrationAngle",
            "NVisiblePellets",
            "MeatConcentrationWeight",
            "MeatConcentrationAngle",
            "NVisibleMeat",
            "ClosestBibiteR",
            "ClosestBibiteG",
            "ClosestBibiteB",
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

        public static readonly NeuronType[] OUTPUT_TYPES = new NeuronType[] {
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
