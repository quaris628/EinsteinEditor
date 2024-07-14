using Einstein.config.bibiteVersions;
using LibraryFunctionReplacements;
using phi.other;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseNeuron : DynamicHoldable
    {
        public static readonly Color DEFAULT_COLOR_GROUP = Color.FromArgb(225, 225, 225);

        private int _index;
        public int Index {
            get { return _index; }
            protected set {
                _index = validateIndex(value);
                FlagChange();
            }
        }

        // TODO make setting type public, and when I do that
        // validate that you can't set the type to (for example)
        // Input when index is in the hidden neuron range
        private NeuronType _type;
        public NeuronType Type {
            get
            {
                return _type;
            }
            protected set
            {
                _type = value;
                FlagChange();
            }

        }
        private string _description;

        private float _bias;
        public float Bias
        {
            get { return _bias; }
            set
            {
                _bias = value;
                FlagChange();
            }
        }

        private float _lastInput;
        public float LastInput
        {
            get { return _lastInput; }
            set
            {
                _lastInput = value;
                FlagChange();
            }
        }

        private float _value;
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                FlagChange();
            }
        }

        // IMPORTANT if this neuron is in a brain,
        // then use Brain's UpdateNeuronDescription method instead of setting the description directly.
        // Otherwise some Brain description stuff will start having problems.
        public string Description {
            get { return _description; }
            set {
                _description = validateDescription(value);
                FlagChange();
            }
        }

        private Color _colorGroup;
        public Color ColorGroup
        {
            get { return _colorGroup; }
            set
            {
                _colorGroup = value;
                FlagChange();
            }
        }

        // description, type, and even index can change, but version must be immutable
        public BibiteVersion BibiteVersion { get; private set; }

        protected BaseNeuron(BibiteVersion bibiteVersion)
        {
            BibiteVersion = bibiteVersion;
            ColorGroup = DEFAULT_COLOR_GROUP;
        }

        // Description will be the type name
        public BaseNeuron(int index, NeuronType type, BibiteVersion bibiteVersion) : this(index, type,
            0f, Enum.GetName(typeof(NeuronType), type), bibiteVersion) { }

        public BaseNeuron(int index, NeuronType type, float bias, string description, BibiteVersion bibiteVersion) : this(bibiteVersion)
        {
            Index = index;
            Type = type;
            Bias = bias;
            Value = bibiteVersion.IsConstantInputNeuron(index) ? 1f : 0f;
            LastInput = 0f;
            Description = description;
            ColorGroup = DEFAULT_COLOR_GROUP;
            if (IsInput() && IsOutput())
            {
                // This is Leo's fault. Don't blame me.
                throw new InputOutputAmbiguityException(
                    "It is unclear whether this neuron is an input or output neuron. " +
                    "Its type (" + Type + ") indicates it is an input neuron, " +
                    "but its index (" + Index + ") indicates it is an output neuron.");
            }
        }

        public bool IsInput()
        {
            return Type == NeuronType.Input;
        }

        public bool IsOutput()
        {
            return BibiteVersion.OUTPUT_NODES_INDEX_MIN <= Index
                && Index <= BibiteVersion.OUTPUT_NODES_INDEX_MAX;
        }

        public bool IsHidden()
        {
            return !IsInput() && !IsOutput();
        }

        public override bool Equals(object obj)
        {
            if (this == obj) { return true; }
            if (!(obj is BaseNeuron)) { return false; }
            BaseNeuron objNeuron = (BaseNeuron)obj;
            return this.Index == objNeuron.Index && this.Type == objNeuron.Type;
        }

        public override string ToString()
        {
            return Description + " : " + Type.ToString() + " [i" + Index + " v" + BibiteVersion.ToString() + "]";
        }

        public static string GetDefaultDescription(int index) { return "Hidden" + index; }

        private int validateIndex(int value)
        {
            if (value < 0)
            {
                throw new InvalidIndexException(
                    "Index ('" + value + "') cannot be negative");
            }
            return value;
        }

        private string validateDescription(string value)
        {
            if (value == null)
            {
                return GetDefaultDescription(Index);
            }
            else if (!value.All(isValidCharForDesc))
            {
                throw new InvalidDescriptionException(
                    "Neuron descriptions must be alphanumeric (except _ and -)");
            }
            return value;
        }

        public static bool isValidCharForDesc(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }

        public string getBiasAsStringForUI()
        {
            return CustomNumberParser.FloatToString(Bias, int.MaxValue,
                BibiteConfigVersionIndependent.NEURON_BIAS_MAX_DECIMALS);
        }
        public void setBiasAsStringForUI(string bias)
        {
            Bias = CustomNumberParser.StringToFloat(bias);
        }
        public string getValueAsStringForUI()
        {
            return CustomNumberParser.FloatToString(Value, int.MaxValue,
                BibiteConfigVersionIndependent.NEURON_VALUE_MAX_DECIMALS);
        }
        public void setValueAsStringForUI(string value)
        {
            Value = CustomNumberParser.StringToFloat(value);
        }
    }

    public class InvalidIndexException : BrainException
    {
        public const string TITLE = "Invalid index";
        public InvalidIndexException() : base(TITLE) { }
        public InvalidIndexException(string message) : base(TITLE, message) { }
        public InvalidIndexException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }
    public class InvalidDescriptionException : BrainException
    {
        public const string TITLE = "Invalid description";
        public InvalidDescriptionException() : base(TITLE) { }
        public InvalidDescriptionException(string message) : base(TITLE, message) { }
        public InvalidDescriptionException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }
    public class InputOutputAmbiguityException : BrainException
    {
        public const string TITLE = "Input/Output Ambiguity";
        public InputOutputAmbiguityException() : base(TITLE) { }
        public InputOutputAmbiguityException(string message) : base(TITLE, message) { }
        public InputOutputAmbiguityException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }

}
