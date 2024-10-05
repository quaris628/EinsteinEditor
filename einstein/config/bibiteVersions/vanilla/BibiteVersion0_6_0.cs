using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Einstein.model.json.JsonNeuron;

namespace Einstein.config.bibiteVersions.vanilla
{
    public class BibiteVersion0_6_0 : BibiteVanillaVersion
    {
        internal static readonly BibiteVersion0_6_0 INSTANCE = new BibiteVersion0_6_0();

        private BibiteVersion0_6_0(): base(609)
        {
            VERSION_NAME = "0.6.0";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 31;
            OUTPUT_NODES_INDEX_MIN = 32;
            OUTPUT_NODES_INDEX_MAX = 46;
            HIDDEN_NODES_INDEX_MIN = 47;
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
            return bibitesVersionName.Equals("0.6")
                    || bibitesVersionName.Equals("0.6.0");
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

        // Changes from 0.6a18:
        // Nothing.

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new ArgumentException($"source brain version ({brain.BibiteVersion.VERSION_NAME}) does not match the converting version ({VERSION_NAME})");
            }
            // To 0.6a18
            // deep copy with no changes
            return new JsonBrain(brain, V0_6_0a18);
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new ArgumentException($"source brain version ({brain.BibiteVersion.VERSION_NAME}) does not match the converting version ({VERSION_NAME})");
            }
            throw new NoSuchVersionException("There is no supported version higher than " + VERSION_NAME);
        }

        #endregion Converting Between Versions
    }
}
