using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseSynapse
    {
        public BaseNeuron From { get; set; }
        public BaseNeuron To { get; set; }
        public float Strength { get; set; }

        protected BaseSynapse() { }

        public BaseSynapse(BaseNeuron from, BaseNeuron to, float strength)
        {
            if (from == null || to == null) { throw new ArgumentNullException(); }
            if (from.Index == to.Index) { throw new SameNeuronException(
                "Cannot create a synapse that connects any neuron (index " +
                from.Index + " ) to itself."); }
            strength = Math.Max(-10f, Math.Min(strength, 10f));

            From = from;
            To = to;
            Strength = strength;
        }

        public override string ToString()
        {
            return From.ToString() + " --(x" + Math.Round(Strength, 2) + ")--> " + To.ToString();
        }

        public virtual string GetSave() { throw new NotSupportedException(); }
    }

    public class SameNeuronException : BrainException
    {
        public const string TITLE = "Contains duplicate";
        public SameNeuronException() : base(TITLE) { }
        public SameNeuronException(string message) : base(TITLE, message) { }
        public SameNeuronException(string message, Exception innerException)
            : base(TITLE, message, innerException) { }
    }
}
