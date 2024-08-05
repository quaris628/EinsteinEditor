﻿using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Einstein.model.json.JsonNeuron;

namespace Einstein.config.bibiteVersions
{
    public class BibiteVersion0_6_0a10thru12 : BibiteVersion
    {
        internal static readonly BibiteVersion0_6_0a10thru12 INSTANCE = new BibiteVersion0_6_0a10thru12();

        private BibiteVersion0_6_0a10thru12(): base(62)
        {
            VERSION_NAME = "0.6.0a 10 thru 12";

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
                NeuronType.TanH,
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

        protected override bool IsMatchForVersionName(string bibitesVersionName)
        {
            if (!StringHasPrefix(bibitesVersionName, "0.6a")
                && !StringHasPrefix(bibitesVersionName, "0.6.0a"))
            {
                return false;
            }

            foreach (string alphaNumber in new string[] { "10", "11", "12" })
            {
                if (StringHasPrefix(bibitesVersionName, "0.6a" + alphaNumber)
                    || StringHasPrefix(bibitesVersionName, "0.6.0a" + alphaNumber))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool HasBiases()
        {
            return false;
        }

        public override SynapseFiringCalcMethod GetSynapseOrderCalcMethod()
        {
            return SynapseFiringCalcMethod.InOrder;
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

        // Changes from 0.6a5thru9:
        // deltaTime calculation now uses a brainUpdateFactor and tps

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            return new JsonBrain(V0_6_0a5thru9);
        }
        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.6a13thru15
            // Remove constant node and replace connections with biases, and shift all indexes down 1

            BaseBrain brainOut = new JsonBrain(V0_6_0a13thru15);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                if (neuron.Index == 0) // don't copy constant over
                {
                    continue;
                }

                // decrement each index by 1
                int newIndex = neuron.Index - 1;

                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, 0f, neuron.Description, V0_6_0a0thru4);
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                if (synapse.From.Index == 0) // if from constant
                {
                    int newToIndex = synapse.To.Index - 1;
                    BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                    // convert to bias
                    newTo.Bias = synapse.Strength;
                }
                else
                {
                    // decrement both indexes by 1
                    int newToIndex = synapse.To.Index - 1;
                    int newFromIndex = synapse.From.Index - 1;
                    BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                    BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                    brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
                }
            }

            return brainOut;
        }
    }
}
