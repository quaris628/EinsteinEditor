using Einstein.config.bibiteVersions.vanilla;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Einstein.model.json.JsonNeuron;
using static Einstein.ui.editarea.NeuronValueCalculator;

namespace Einstein.config.bibiteVersions.strafe
{
    public class BibiteVersion0_5_DietStrengthDefence_v1 : BibiteModdedVersion
    {
        internal static readonly BibiteVersion0_5_DietStrengthDefence_v1 INSTANCE = new BibiteVersion0_5_DietStrengthDefence_v1();

        private BibiteVersion0_5_DietStrengthDefence_v1(): base(BibiteVersion0_5.INSTANCE)
        {
            VERSION_NAME = "0.5 DietStrengthDefence v1";

            INPUT_NODES_INDEX_MIN = 0;
            INPUT_NODES_INDEX_MAX = 35;
            OUTPUT_NODES_INDEX_MIN = 36;
            OUTPUT_NODES_INDEX_MAX = 50;
            HIDDEN_NODES_INDEX_MIN = 51;
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
                "DietBibite", // Added by this mod
                "StrengthBibite", // Added by this mod
                "DefenceBibite", // Added by this mod
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

            // Identical to vanilla
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

            // Identical to vanilla
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
            return Regex.IsMatch(bibitesVersionName, "0\\.5.*modded: ?DietStrengthDefence ?v1");
        }

        #endregion Version Name Matching

        // Identical to vanilla
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

        // Identical to vanilla
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
            // always set inov
            SetNeuronDiagramPositionInInov(fields, x, y);
        }

        #endregion Neuron diagram positions

        #region Converting Between Versions

        internal override BaseBrain CreateCopyFromVanilla(BaseBrain brain)
        {
            if (brain.BibiteVersion != GetVanilla())
            {
                throw new InvalidOperationException("brain version must be the mod's base vanilla version");
            }
            // Add 3 new Input neurons, and shift indexes to accomodate them

            BaseBrain brainOut = new JsonBrain(this);
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertVanillaNeuronIndexToModded(neuron.Index);
                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, 0f, neuron.Description, GetVanilla());
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertVanillaNeuronIndexToModded(synapse.To.Index);
                int newFromIndex = ConvertVanillaNeuronIndexToModded(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            // DietBibite
            brainOut.Add(new JsonNeuron(33, NeuronType.Input, 0f, this.DESCRIPTIONS[33], this));
            // StrengthBibite
            brainOut.Add(new JsonNeuron(34, NeuronType.Input, 0f, this.DESCRIPTIONS[34], this));
            // DefenceBibite
            brainOut.Add(new JsonNeuron(35, NeuronType.Input, 0f, this.DESCRIPTIONS[35], this));

            return brainOut;
        }

        private int ConvertVanillaNeuronIndexToModded(int vanillaIndex)
        {
            if (0 <= vanillaIndex && vanillaIndex <= 32)
            {
                return vanillaIndex;
            }
            else if (33 <= vanillaIndex)
            {
                return vanillaIndex + 3;
            }
            else
            {
                throw new ArgumentException("index cannot be negative");
            }
        }

        internal override BaseBrain CreateVanillaCopyOf(BaseBrain brain)
        {
            if (brain.BibiteVersion != this)
            {
                throw new InvalidOperationException("brain version must match");
            }
            // Remove the 3 new Input neurons, and shift indexes to fill the holes

            BaseBrain brainOut = new JsonBrain(GetVanilla());
            foreach (BaseNeuron neuron in brain.Neurons)
            {
                // update each index
                int newIndex = ConvertModdedNeuronIndexToVanilla(neuron.Index);

                JsonNeuron jn = new JsonNeuron(newIndex, neuron.Type, 0f, neuron.Description, GetVanilla());
                jn.DiagramX = ((JsonNeuron)neuron).DiagramX;
                jn.DiagramY = ((JsonNeuron)neuron).DiagramY;
                jn.ColorGroup = ((JsonNeuron)neuron).ColorGroup;

                brainOut.Add(jn);
            }
            foreach (BaseSynapse synapse in brain.Synapses)
            {
                int newToIndex = ConvertModdedNeuronIndexToVanilla(synapse.To.Index);
                int newFromIndex = ConvertModdedNeuronIndexToVanilla(synapse.From.Index);
                BaseNeuron newTo = brainOut.GetNeuron(newToIndex);
                BaseNeuron newFrom = brainOut.GetNeuron(newFromIndex);
                brainOut.Add(new JsonSynapse((JsonNeuron)newFrom, (JsonNeuron)newTo, synapse.Strength));
            }

            return brainOut;
        }

        private int ConvertModdedNeuronIndexToVanilla(int moddedIndex)
        {
            if (0 <= moddedIndex && moddedIndex <= 32)
            {
                return moddedIndex;
            }
            else if (33 == moddedIndex)
            {
                throw new CannotConvertException("This brain contains a DietBibite neuron, which does not exist in version " + GetVanilla().VERSION_NAME);
            }
            else if (34 == moddedIndex)
            {
                throw new CannotConvertException("This brain contains a StrengthBibite neuron, which does not exist in version " + GetVanilla().VERSION_NAME);
            }
            else if (35 == moddedIndex)
            {
                throw new CannotConvertException("This brain contains a DefenceBibite neuron, which does not exist in version " + GetVanilla().VERSION_NAME);
            }
            else if (36 <= moddedIndex)
            {
                return moddedIndex - 3;
            }
            else
            {
                throw new ArgumentException("index cannot be negative");
            }
        }

        #endregion Converting Between Versions
    }
}
