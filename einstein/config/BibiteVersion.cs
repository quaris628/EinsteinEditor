using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using LibraryFunctionReplacements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Einstein.model.json.JsonNeuron;

namespace Einstein.config.bibiteVersions
{
    public abstract class BibiteVersion : IComparable<BibiteVersion>
    {
        #region Version Instances (Static)

        // What the version is when you start the editor
        public static readonly BibiteVersion DEFAULT_VERSION = V0_5;

        public static readonly BibiteVersion V0_6_0a13thru15 = BibiteVersion0_6_0a13thru15.INSTANCE;
        public static readonly BibiteVersion V0_6_0a5thru9 = BibiteVersion0_6_0a5thru9.INSTANCE;
        public static readonly BibiteVersion V0_6_0a10thru12 = BibiteVersion0_6_0a10thru12.INSTANCE;
        public static readonly BibiteVersion V0_6_0a0thru4 = BibiteVersion0_6_0a0thru4.INSTANCE;
        public static readonly BibiteVersion V0_5 = BibiteVersion0_5.INSTANCE;
        public static readonly BibiteVersion V0_4 = BibiteVersion0_4.INSTANCE;

        protected static readonly BibiteVersion[] ALL_VERSIONS = new BibiteVersion[] {
                V0_6_0a13thru15,
                V0_6_0a10thru12,
                V0_6_0a5thru9,
                V0_6_0a0thru4,
                V0_5,
                V0_4
            };

        #endregion Version Instances (Static)

        #region Properties

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
        protected NeuronType[] neuronTypes;

        #endregion Properties

        protected BibiteVersion(int versionId)
        {
            this.versionId = versionId;
        }

        #region Version Name Matching

        public static BibiteVersion FromName(string versionName)
        {
            // could always do more complex version number handling here,
            // in case we need that flexibility for weird scenarios
            foreach (BibiteVersion version in ALL_VERSIONS)
            {
                if (version.IsMatchForVersionName(versionName))
                {
                    return version;
                }
            }
            throw new NoSuchVersionException($"Unrecognized version '{versionName}'");
        }

        protected virtual bool IsMatchForVersionName(string bibitesVersionName)
        {
            return StringHasPrefix(bibitesVersionName, VERSION_NAME);
        }

        protected bool StringHasPrefix(string haystack, string prefix)
        {
            return haystack.Length >= prefix.Length
                && haystack.Substring(0, prefix.Length).Equals(prefix);
        }

        #endregion Version Name Matching

        #region Brain Calculations

        public virtual bool HasBiases()
        {
            return true;
        }

        public bool IsConstantInputNeuron(int index)
        {
            return !HasBiases() && index == 0;
        }

        public virtual DeltaTimeCalcMethod GetDeltaTimeCalcMethod()
        {
            return DeltaTimeCalcMethod.BrainUpdateFactorOverTps;
        }

        public enum DeltaTimeCalcMethod
        {
            SimSpeed,
            BrainUpdateFactorOverTps,
        }

        #endregion Brain Calculations

        #region Neuron diagram positions

        // inov is the old system for storing position information,
        // but the Inov field started being used for something in 0.6
        // and I didn't want the position data to interfere with it,
        // so I changed einstein to tag on the position data to the end of descriptions instead.
        // However there will be old 0.5 (and a few 0.6 alpha) bibites that already have Inov set.

        public virtual bool GetNeuronDiagramPositionFromRawJsonFields(RawJsonFields fields, ref int x, ref int y)
        {
            return GetNeuronDiagramPositionFromDescription(fields, ref x, ref y);
            // only if version is 0.6.a or below, try falling back to inov
        }

        public virtual void SetNeuronDiagramPositionInRawJsonFields(RawJsonFields fields, int x, int y)
        {
            SetNeuronDiagramPositionInDesc(fields, x, y);
            // in 0.5 and below, set all inovs
            // in 0.6.0a, set inovs only for hidden neurons
            // plan to not set inov for future versions
        }

        protected bool GetNeuronDiagramPositionFromInov(RawJsonFields fields, ref int x, ref int y)
        {
            if (fields.inov != 0)
            {
                x = fields.inov >> 16;
                y = (fields.inov & 0xffff) - 32768;
                return true;
            }
            return false;
        }

        protected virtual void SetNeuronDiagramPositionInInov(RawJsonFields fields, int x, int y)
        {
            fields.inov = (x << 16) | (y + 32768);
        }

        protected bool GetNeuronDiagramPositionFromDescription(RawJsonFields fields, ref int x, ref int y)
        {
            if (fields.HasDescPiece(RawJsonFields.DescPiece.DiagramPosX)
                && fields.HasDescPiece(RawJsonFields.DescPiece.DiagramPosY))
            {
                x = DescStrPieceToDiagramPosition(fields.GetDescPiece(RawJsonFields.DescPiece.DiagramPosX));
                y = DescStrPieceToDiagramPosition(fields.GetDescPiece(RawJsonFields.DescPiece.DiagramPosY));
                return true;
            }
            return false;
        }

        protected virtual void SetNeuronDiagramPositionInDesc(RawJsonFields fields, int x, int y)
        {
            string xPiece = DiagramPositionToDescStrPiece(x);
            string yPiece = DiagramPositionToDescStrPiece(y);
            fields.SetDescPiece(RawJsonFields.DescPiece.DiagramPosX, xPiece);
            fields.SetDescPiece(RawJsonFields.DescPiece.DiagramPosY, yPiece);
        }

        protected const string BASE_36_DIGITS = "0123456789abcdefghijklmnopqrstuvwxyz";
        // inclusive
        protected const int MAX_POS = (36 * 36 * 36) / 2 - 1;
        protected const int MIN_POS = -(36 * 36 * 36) / 2;

        protected virtual string DiagramPositionToDescStrPiece(int pos)
        {
            if (!(MIN_POS <= pos && pos <= MAX_POS))
            {
                throw new ArgumentException($"Diagram position {pos} out of range");
            }

            int unsignedIntToConvert = pos - MIN_POS;
            int digit1 = unsignedIntToConvert / (36 * 36);
            unsignedIntToConvert -= digit1 * (36 * 36);
            int digit2 = unsignedIntToConvert / 36;
            unsignedIntToConvert -= digit2 * 36;
            int digit3 = unsignedIntToConvert;

            StringBuilder s = new StringBuilder();
            s.Append(BASE_36_DIGITS[digit1]);
            s.Append(BASE_36_DIGITS[digit2]);
            s.Append(BASE_36_DIGITS[digit3]);
            return s.ToString();
        }

        protected virtual int DescStrPieceToDiagramPosition(string base36Str)
        {
            if (base36Str.Length != 3)
            {
                throw new ArgumentException("base 36 string must be 3 chars long to convert to diagram position");
            }

            int digit1 = BASE_36_DIGITS.IndexOf(base36Str[0]);
            int digit2 = BASE_36_DIGITS.IndexOf(base36Str[1]);
            int digit3 = BASE_36_DIGITS.IndexOf(base36Str[2]);

            return MIN_POS + (digit1 * 36 * 36 + digit2 * 36 + digit3);
        }

        #endregion Neuron diagram positions

        #region Neuron Getters

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
                        _inputNeurons[i - INPUT_NODES_INDEX_MIN] = new JsonNeuron(i, NeuronType.Input, 0f, DESCRIPTIONS[i], this);
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
                        _outputNeurons[i - OUTPUT_NODES_INDEX_MIN] = new JsonNeuron(i, GetOutputNeuronType(i), 0f, DESCRIPTIONS[i], this);
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
                    _hiddenNeurons = new BaseNeuron[neuronTypes.Length - 1];
                    int index = HIDDEN_NODES_INDEX_MIN;
                    foreach (NeuronType neuronType in neuronTypes)
                    {
                        if (neuronType == NeuronType.Input) { continue; }
                        _hiddenNeurons[index - HIDDEN_NODES_INDEX_MIN] = new JsonNeuron(index++, neuronType, 0f, this);
                    }
                }
                return _hiddenNeurons;
            }
        }

        #endregion Neuron Getters

        #region Generic Overrides

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

        #endregion Generic Overrides

        #region Converting Between Versions

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

        #endregion Converting Between Versions
    }
}

