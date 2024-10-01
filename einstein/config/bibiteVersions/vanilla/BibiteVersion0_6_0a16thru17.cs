using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Einstein.model.json.JsonNeuron;
using static Einstein.ui.editarea.NeuronValueCalculator;

namespace Einstein.config.bibiteVersions.vanilla
{
    public class BibiteVersion0_6_0a16thru17 : BibiteVanillaVersion
    {
        internal static readonly BibiteVersion0_6_0a16thru17 INSTANCE = new BibiteVersion0_6_0a16thru17();

        private BibiteVersion0_6_0a16thru17(): base(64)
        {
            VERSION_NAME = "0.6.0a16 thru 17";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 32;
            OUTPUT_NODES_INDEX_MIN = 33;
            OUTPUT_NODES_INDEX_MAX = 48;
            HIDDEN_NODES_INDEX_MIN = 49;
            HIDDEN_NODES_INDEX_MAX = int.MaxValue;

            DESCRIPTIONS = new string[] {
                // ----- Inputs -----
                "EnergyRatio",
                "Maturity",
                "LifeRatio",
                "Fullness",
                "Speed",
                "IsGrabbing",
                "AttackedDamage",
                "EggStored",
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
                "EggProduction",
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
                NeuronType.TanH, // Accelerate
                NeuronType.TanH, // Rotate
                NeuronType.TanH, // Herding
                NeuronType.TanH, // EggProduction
                NeuronType.Sigmoid, // Want2Lay
                NeuronType.TanH, // Want2Eat - Changed to a TanH (instead of Sigmoid)
                NeuronType.Sigmoid, // Digestion
                NeuronType.TanH, // Grab
                NeuronType.Sigmoid, // ClkReset
                NeuronType.ReLu, // PhereOut1
                NeuronType.ReLu, // PhereOut2
                NeuronType.ReLu, // PhereOut3
                NeuronType.Sigmoid, // Want2Grow
                NeuronType.Sigmoid, // Want2Heal
                NeuronType.Sigmoid, // Want2Attack
                NeuronType.TanH, // ImmuneSystem
            };

            neuronTypes = new NeuronType[]
            {
                NeuronType.Input,
                NeuronType.Sigmoid,
                NeuronType.Linear,
                NeuronType.TanH,
                NeuronType.Sine,
                NeuronType.ReLu,
                NeuronType.Gaussian,
                NeuronType.Latch,
                NeuronType.Differential,
                NeuronType.Abs,
                NeuronType.Mult,
                NeuronType.Integrator,
                NeuronType.Inhibitory,
            };
        }

        #region Version Name Matching

        protected override bool IsMatchForVersionName(string bibitesVersionName)
        {
            if (!StringHasPrefix(bibitesVersionName, "0.6a")
                && !StringHasPrefix(bibitesVersionName, "0.6.0a"))
            {
                return false;
            }

            foreach (string alphaNumber in new string[] { "16", "17" })
            {
                if (bibitesVersionName.Equals("0.6a" + alphaNumber)
                    || bibitesVersionName.Equals("0.6.0a" + alphaNumber))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Version Name Matching

        #region Neuron diagram positions

        public override bool GetNeuronDiagramPositionFromRawJsonFields(RawJsonFields fields, ref int x, ref int y)
        {
            // fall back to inov if it's not in the description
            return GetNeuronDiagramPositionFromDescription(fields, ref x, ref y)
                || GetNeuronDiagramPositionFromInov(fields, ref x, ref y);
        }
        public override void SetNeuronDiagramPositionInRawJsonFields(RawJsonFields fields, int x, int y)
        {
            SetNeuronDiagramPositionInDesc(fields, x, y);
            // set inov for diagram position if and only if the neuron type is hidden
            if (new JsonNeuron(fields, INSTANCE).IsHidden())
            {
                SetNeuronDiagramPositionInInov(fields, x, y);
            }
            else
            {
                fields.inov = fields.index + 1;
            }
        }

        #endregion Neuron diagram positions

        #region Converting Between Versions

        // Changes from 0.6a13thru15:
        // None (except for synapse order calculations)

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new ArgumentException($"source brain version ({brain.BibiteVersion.VERSION_NAME}) does not match the converting version ({VERSION_NAME})");
            }
            // To 0.6a13thru15
            // Want2Eat needs to have its type changed to a Sigmoid
            JsonBrain newBrain = new JsonBrain(brain, V0_6_0a13thru15);
            newBrain.GetNeuron(38).Type = NeuronType.Sigmoid;
            return newBrain;
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            throw new NoSuchVersionException("There is no supported version higher than " + VERSION_NAME);
        }

        #endregion Converting Between Versions
    }
}
