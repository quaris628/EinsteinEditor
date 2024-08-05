using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Einstein.model.json.JsonNeuron;
using static Einstein.ui.editarea.NeuronValueCalculator;

namespace Einstein.config.bibiteVersions
{
    public class BibiteVersion0_6_0a0thru4 : BibiteVersion
    {
        internal static readonly BibiteVersion0_6_0a0thru4 INSTANCE = new BibiteVersion0_6_0a0thru4();

        private BibiteVersion0_6_0a0thru4(): base(60)
        {
            VERSION_NAME = "0.6.0a 0 thru 4";

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

            foreach (char alphaNumber in "01234")
            {
                if (StringHasPrefix(bibitesVersionName, "0.6a" + alphaNumber)
                    || StringHasPrefix(bibitesVersionName, "0.6.0a" + alphaNumber))
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

        // inov is the old system for storing position information,
        // but the Inov field started being used for something in 0.6
        // and I didn't want the position data to interfere with it,
        // so I changed einstein to tag on the position data to the end of descriptions instead.
        // However there will be old 0.5 (and a few 0.6 alpha) bibites that already have Inov set.

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            // To 0.5
            // deep copy with no changes (preserve Inov)
            return new JsonBrain(brain, V0_5);
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.6.0a5thru12
            // Add 2 new IO neurons, and shift indexes to accomodate them

            BaseBrain brainOut = new JsonBrain(V0_6_0a5thru9);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertNeuronIndexTo0_6_0a5thru12(neuron.Index);
                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, 0f, neuron.Description, V0_6_0a5thru9);
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertNeuronIndexTo0_6_0a5thru12(synapse.To.Index);
                int newFromIndex = ConvertNeuronIndexTo0_6_0a5thru12(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            // EggsStored
            brainOut.Add(new JsonNeuron(8, NeuronType.Input, 0f, V0_6_0a5thru9.DESCRIPTIONS[8], V0_6_0a5thru9));
            // EggProduction
            brainOut.Add(new JsonNeuron(37, NeuronType.TanH, 0f, V0_6_0a5thru9.DESCRIPTIONS[37], V0_6_0a5thru9));

            return brainOut;
        }

        private int ConvertNeuronIndexTo0_6_0a5thru12(int index)
        {
            if (0 <= index && index <= 7)
            {
                return index;
            }
            else if (8 <= index && index <= 35)
            {
                return index + 1;
            }
            else if (36 <= index)
            {
                return index + 2;
            }
            else
            {
                throw new ArgumentException("index cannot be negative");
            }
        }

        #endregion Converting Between Versions
    }
}
