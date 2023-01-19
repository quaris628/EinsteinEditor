using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseSynapse
    {
        private BaseNeuron _from;
        private BaseNeuron _to;
        private float _strength;
        public BaseNeuron From {
            get { return _from; }
            set { _from = validateFrom(value); }
        }
        public BaseNeuron To
        {
            get { return _to; }
            set { _to = validateTo(value); }
        }

        public float Strength {
            get { return _strength; }
            set { _strength = validateStrength(value); }
        }

        protected BaseSynapse() { }

        public BaseSynapse(BaseNeuron from, BaseNeuron to, float strength)
        {
            From = from;
            To = to;
            Strength = strength;
        }

        public override string ToString()
        {
            return From.ToString() + " --(x" + Math.Round(Strength, 2) + ")--> " + To.ToString();
        }

        public virtual string GetSave() { throw new NotSupportedException(); }

        private BaseNeuron validateFrom(BaseNeuron value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            if (To != null && value.Index == To.Index)
            {
                throw new SameNeuronException(
                    "Cannot create a synapse that connects a neuron to itself.");
            }
            return value;
        }
        private BaseNeuron validateTo(BaseNeuron value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            if (From != null && value.Index == From.Index)
            {
                throw new SameNeuronException(
                    "Cannot create a synapse that connects a neuron to itself.");
            }
            return value;
        }
        private float validateStrength(float value)
        {
            value = Math.Max(BibiteVersionConfig.SYNAPSE_STRENGTH_MIN,
                Math.Min(value,
                BibiteVersionConfig.SYNAPSE_STRENGTH_MAX));
            return value;
        }
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
