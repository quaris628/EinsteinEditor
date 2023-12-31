using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config
{
    public abstract class BibiteVersion : IComparable<BibiteVersion>
    {
        #region Version Instances - Static

        public static readonly BibiteVersion V0_6 = BibiteVersion0_6.INSTANCE;
        public static readonly BibiteVersion V0_5 = BibiteVersion0_5.INSTANCE;
        public static readonly BibiteVersion V0_4 = BibiteVersion0_4.INSTANCE;

        // What the version is when you start the editor
        public static readonly BibiteVersion DEFAULT_VERSION = V0_6;

        // could always do more complex version number handling here,
        // in case we need that flexibility for weird scenarios

        public static BibiteVersion FromName(string versionName)
        {
            if (V0_6.IsMatchForVersionName(versionName))
            {
                return V0_6;
            }
            else if (V0_5.IsMatchForVersionName(versionName))
            {
                return V0_5;
            }
            else if (V0_4.IsMatchForVersionName(versionName))
            {
                return V0_4;
            }
            else
            {
                throw new ArgumentException($"Unrecognized version '{versionName}'");
            }
        }

        #endregion Version Instances - Static

        #region Data

        // used to order the versions
        private int versionId;

        public string VERSION_NAME { get; protected set; }
        
        public int INPUT_NODES_INDEX_MIN { get; protected set; }
        public int INPUT_NODES_INDEX_MAX { get; protected set; }
        public int OUTPUT_NODES_INDEX_MIN { get; protected set; }
        public int OUTPUT_NODES_INDEX_MAX { get; protected set; }
        public int HIDDEN_NODES_INDEX_MIN { get; protected set; }
        public int HIDDEN_NODES_INDEX_MAX { get; protected set; }

        public string[] DESCRIPTIONS { get; protected set; }

        protected NeuronType[] outputTypes;

        #endregion Data

        protected BibiteVersion(int versionId)
        {
            this.versionId = versionId;
        }

        protected virtual bool IsMatchForVersionName(string bibitesVersionName)
        {
            return bibitesVersionName.Substring(0, VERSION_NAME.Length).Equals(VERSION_NAME);
        }

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

        #endregion Getters

        #region Overrides

        public int CompareTo(BibiteVersion other)
        {
            return versionId.CompareTo(other.versionId);
        }

        public static bool operator >(BibiteVersion v1, BibiteVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(BibiteVersion v1, BibiteVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >=(BibiteVersion v1, BibiteVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <=(BibiteVersion v1, BibiteVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public override string ToString()
        {
            return VERSION_NAME;
        }

        #endregion Overrides

        #region Converting

        public BaseBrain CreateConvertedCopyOf(BaseBrain brain, BibiteVersion targetVersion)
        {
            if (brain.BibiteVersion == targetVersion)
            {
                return brain;
            }
            else if (brain.BibiteVersion < targetVersion)
            {
                // convert upwards
                do
                {
                    brain = brain.BibiteVersion.CreateVersionUpCopyOf(brain);
                }
                while (brain.BibiteVersion < targetVersion);
                return brain;
            }
            else // if (brain.BibiteVersion > targetVersion)
            {
                // convert downwards
                do
                {
                    brain = brain.BibiteVersion.CreateVersionDownCopyOf(brain);
                }
                while (brain.BibiteVersion > targetVersion);
                return brain;
            }
        }

        protected abstract BaseBrain CreateVersionUpCopyOf(BaseBrain brain);

        protected abstract BaseBrain CreateVersionDownCopyOf(BaseBrain brain);

        #endregion Converting
    }
}

