using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public abstract class BaseNeuron
    {
        // TODO add verification to setters on properties (on synapses and brain (?) too)
        public int Index { get; protected set; }
        public NeuronType Type { get; set; }
        public string Description { get; set; }

        protected BaseNeuron() { }

        public BaseNeuron(int index, NeuronType type, string description)
        {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("Index", index, "cannot be negative"); }
            if (description == null) {
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
                throw new InputOutputConflictException(
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
            return VersionConfig.OUTPUT_NODES_INDEX_MIN <= Index
                && Index <= VersionConfig.OUTPUT_NODES_INDEX_MAX;
        }

        public override string ToString()
        {
            return Description + " : " + Type.ToString();
        }

        public abstract string GetSave();

        public static string GetDefaultDescription(int index) { return "Hidden" + index; }
    }
    public class InvalidDescriptionException : Exception {
        public InvalidDescriptionException() : base() { }
        public InvalidDescriptionException(string message) : base(message) { }
        public InvalidDescriptionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    public class InputOutputConflictException : Exception
    {
        public InputOutputConflictException() : base() { }
        public InputOutputConflictException(string message) : base(message) { }
        public InputOutputConflictException(string message, Exception innerException)
            : base(message, innerException) { }
    }

}
