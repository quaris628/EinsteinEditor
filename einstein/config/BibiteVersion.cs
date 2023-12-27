using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config
{
    public class BibiteVersion
    {
        #region Data
        // any version that starts with these characters will be considered a version match
        // e.g. "version": "0.5.1" matches VERSION_NAME = "0.5"
        // note this means you could do 0.6 to cover any 0.6 version,
        // OR you could have a separate 0.6.1, 0.6.2, etc
        public string VERSION_NAME { get; private set; }

        public int INPUT_NODES_INDEX_MIN { get; private set; }
        public int INPUT_NODES_INDEX_MAX { get; private set; }
        public int OUTPUT_NODES_INDEX_MIN { get; private set; }
        public int OUTPUT_NODES_INDEX_MAX { get; private set; }
        public int HIDDEN_NODES_INDEX_MIN { get; private set; }
        public int HIDDEN_NODES_INDEX_MAX { get; private set; }

        public string[] DESCRIPTIONS { get; private set; }

        private NeuronType[] outputTypes;

        #endregion Data

        private BibiteVersion() { }

        #region Getters
        
        public NeuronType GetOutputNeuronType(int index)
        {
            return outputTypes[index - OUTPUT_NODES_INDEX_MIN];
        }

        private BaseNeuron[] _inputNeurons = null;
        public IEnumerable<BaseNeuron> InputNeurons
        {
            get
            {
                if (_inputNeurons == null)
                {
                    _inputNeurons = new BaseNeuron[INPUT_NODES_INDEX_MAX - INPUT_NODES_INDEX_MIN + 1];
                    for (int i = INPUT_NODES_INDEX_MIN;
                        i <= INPUT_NODES_INDEX_MAX; i++)
                    {
                        _inputNeurons[i - INPUT_NODES_INDEX_MIN] = new JsonNeuron(i, NeuronType.Input, DESCRIPTIONS[i], this);
                    }
                }
                return _inputNeurons;
            }
        }

        private BaseNeuron[] _outputNeurons = null;
        public IEnumerable<BaseNeuron> OutputNeurons
        {
            get
            {
                if (_outputNeurons == null)
                {
                    _outputNeurons = new BaseNeuron[OUTPUT_NODES_INDEX_MAX - OUTPUT_NODES_INDEX_MIN + 1];
                    for (int i = OUTPUT_NODES_INDEX_MIN;
                        i <= OUTPUT_NODES_INDEX_MAX; i++)
                    {
                        _outputNeurons[i - OUTPUT_NODES_INDEX_MIN] = new JsonNeuron(i, GetOutputNeuronType(i), DESCRIPTIONS[i], this);
                    }
                }
                return _outputNeurons;
            }
        }

        private BaseNeuron[] _hiddenNeurons = null;
        public IEnumerable<BaseNeuron> HiddenNeurons
        {
            get
            {
                if (_hiddenNeurons == null)
                {
                    _hiddenNeurons = new BaseNeuron[Enum.GetValues(typeof(NeuronType)).Length - 1];
                    int index = HIDDEN_NODES_INDEX_MIN;
                    foreach (NeuronType neuronType in Enum.GetValues(typeof(NeuronType)))
                    {
                        if (neuronType == NeuronType.Input) { continue; }
                        _hiddenNeurons[index - HIDDEN_NODES_INDEX_MIN] = new JsonNeuron(index++, neuronType, this);
                    }
                }
                return _hiddenNeurons;
            }
        }

        public override string ToString()
        {
            return VERSION_NAME;
        }

        #endregion Getters

        #region Static Stuff

        /// <summary>
        /// duh
        /// </summary>
        /// <param name="versionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the version name doesn't match any known version</exception>
        public static BibiteVersion FromName(string versionName)
        {
            // could always do more complex version number handling here, in case we need that flexibility for weird scenarios
            if (versionName.Substring(0, V0_5.VERSION_NAME.Length) == V0_5.VERSION_NAME)
            {
                return V0_5;
            }
            else if (versionName.Substring(0, V0_6.VERSION_NAME.Length) == V0_6.VERSION_NAME)
            {
                return V0_6;
            }
            else
            {
                throw new ArgumentException($"Unrecognized version: '{versionName}'");
            }
        }

        // ----- Individual Versions -----

        private static BibiteVersion V0_5 = new BibiteVersion()
        {
            VERSION_NAME = "0.5",
            
            INPUT_NODES_INDEX_MIN = 0,
            INPUT_NODES_INDEX_MAX = 32,
            OUTPUT_NODES_INDEX_MIN = 33,
            OUTPUT_NODES_INDEX_MAX = 47,
            HIDDEN_NODES_INDEX_MIN = 48,
            HIDDEN_NODES_INDEX_MAX = int.MaxValue,

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
            },

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
            },
        };

        private static BibiteVersion V0_6 = new BibiteVersion()
        {
            VERSION_NAME = "0.6",

            INPUT_NODES_INDEX_MIN = 0,
            INPUT_NODES_INDEX_MAX = 33,
            OUTPUT_NODES_INDEX_MIN = 34,
            OUTPUT_NODES_INDEX_MAX = 49,
            HIDDEN_NODES_INDEX_MIN = 50,
            HIDDEN_NODES_INDEX_MAX = int.MaxValue,

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
                "EggStored", // added in 0.6
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
                "EggProduction", // added in 0.6
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
            },

            outputTypes = new NeuronType[]
            {
                NeuronType.TanH,
                NeuronType.TanH,
                NeuronType.TanH,
                NeuronType.TanH, // added in 0.6
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
            },
        };

        // What the version is when you start the editor
        public static readonly BibiteVersion DEFAULT_VERSION = V0_6;

        #endregion Static Stuff
    }
}
