﻿using Einstein.model;
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
    public class BibiteVersion0_6_0a5thru9 : BibiteVanillaVersion
    {
        internal static readonly BibiteVersion0_6_0a5thru9 INSTANCE = new BibiteVersion0_6_0a5thru9();

        private BibiteVersion0_6_0a5thru9(): base(601)
        {
            VERSION_NAME = "0.6.0a 5 thru 9";

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
                "EggStored", // added
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
                "EggProduction", // added
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
                NeuronType.TanH, // added
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

            foreach (char alphaNumber in "56789")
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

        #region Brain Calculations

        public override bool HasBiases()
        {
            return false;
        }

        public override DeltaTimeCalcMethod GetDeltaTimeCalcMethod()
        {
            return DeltaTimeCalcMethod.SimSpeed;
        }

        public override SynapseFiringCalcMethod GetSynapseOrderCalcMethod()
        {
            return SynapseFiringCalcMethod.InOrder;
        }

        #endregion Brain Calculations

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

        // Changes from 0.6a0thru4:
        // - New input neuron EggsStored added at new index 8
        // - New output neuron EggProduction added at new index 37, type is TanH
        //
        // Internal side effects:
        // all indices at 8 to 35 (inclusive) are now 1 higher
        // all indices that were at 36 and above are now 2 higher

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new ArgumentException($"source brain version ({brain.BibiteVersion.VERSION_NAME}) does not match the converting version ({VERSION_NAME})");
            }
            // To 0.6a0 thru 4
            return ConversionRemoveIONeurons(brain, V0_6_0a0thru4, new int[]
            {
                8, // EggStored
                37, // EggProduction
            });
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new ArgumentException($"source brain version ({brain.BibiteVersion.VERSION_NAME}) does not match the converting version ({VERSION_NAME})");
            }
            // To 0.6a10 thru 12
            return new JsonBrain(brain, V0_6_0a10thru12);
        }

        #endregion Converting Between Versions

    }
}
