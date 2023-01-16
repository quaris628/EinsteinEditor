using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseNeuron
    {
        // TODO add verification to setters on properties (on synapses and brain (?) too)
        public int Index { get; protected set; }
        public NeuronType Type { get; set; }
        public string Description { get; set; }

        protected BaseNeuron() { }

        // Description will be the type name
        public BaseNeuron(int index, NeuronType type) : this(index, type,
            Enum.GetName(typeof(NeuronType), type)) { }

        public BaseNeuron(int index, NeuronType type, string description)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("Index", index, "cannot be negative");
            } if (description == null) {
                description = GetDefaultDescription(index);
            } else if (!description.All(char.IsLetterOrDigit)) {
                throw new InvalidDescriptionException(
                    "Neuron descriptions must be alphanumeric");
            }

            Index = index;
            Type = type;
            Description = description;
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
            return BibiteVersionConfig.OUTPUT_NODES_INDEX_MIN <= Index
                && Index <= BibiteVersionConfig.OUTPUT_NODES_INDEX_MAX;
        }

        public bool IsHidden()
        {
            return !IsInput() && !IsOutput();
        }

        public override string ToString()
        {
            return Description + " : " + Type.ToString();
        }

        public virtual string GetSave() { throw new NotSupportedException(); }

        public static string GetDefaultDescription(int index) { return "Hidden" + index; }
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
