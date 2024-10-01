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
    public class BibiteVersion0_6_0a13thru15 : BibiteVanillaVersion
    {
        internal static readonly BibiteVersion0_6_0a13thru15 INSTANCE = new BibiteVersion0_6_0a13thru15();

        private BibiteVersion0_6_0a13thru15(): base(63)
        {
            VERSION_NAME = "0.6.0a 13 thru 15";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 32;
            OUTPUT_NODES_INDEX_MIN = 33;
            OUTPUT_NODES_INDEX_MAX = 48;
            HIDDEN_NODES_INDEX_MIN = 49;
            HIDDEN_NODES_INDEX_MAX = int.MaxValue;

            DESCRIPTIONS = new string[] {
                // ----- Inputs -----
                // Removed Constant
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
                NeuronType.Integrator, // added
                NeuronType.Inhibitory, // added
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

            foreach (string alphaNumber in new string[] { "13", "14", "15" })
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

        // Changes from 0.6a10thru12:
        // - Replaced Constant neuron with Biases
        // - New hidden neurons Integrator (11) and Inhibitory (12)
        //
        // Internal side effects:
        // all indices are now 1 less

        protected override BaseBrain CreateVersionDownCopyOf(BaseBrain brain)
        {
            // To 0.6a10thru12
            // Replace biases with constant node synapses, and shift all indexes up 1

            BaseBrain brainOut = new JsonBrain(V0_6_0a10thru12);
            JsonNeuron constant = new JsonNeuron(0, NeuronType.Input, 0f, V0_6_0a10thru12.DESCRIPTIONS[0], V0_6_0a10thru12);
            brainOut.Add(constant);

            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // increment each index by 1
                int newIndex = neuron.Index + 1;

                JsonNeuron jn = new JsonNeuron(newIndex,
                    neuron.Type,
                    0f,
                    neuron.Description,
                    V0_6_0a10thru12);
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);

                if (neuron.Bias != 0f)
                {
                    // Replace bias with connection to constant
                    brainOut.Add(new JsonSynapse(constant, jn, neuron.Bias));
                }
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                // increment both indexes by 1
                int newToIndex = synapse.To.Index + 1;
                int newFromIndex = synapse.From.Index + 1;
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            return brainOut;
        }

        protected override BaseBrain CreateVersionUpCopyOf(BaseBrain brain)
        {
            // To 0.6a16
            // Want2Eat needs to have its type changed to a TanH
            JsonBrain newBrain = new JsonBrain(brain, V0_6_0a16thru17);
            if (brain.ContainsNeuron(38))
            {
                newBrain.GetNeuron(38).Type = NeuronType.TanH;
            }
            return newBrain;
        }

        #endregion Converting Between Versions
    }
}
