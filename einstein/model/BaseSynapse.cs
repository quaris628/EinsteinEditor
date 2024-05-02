using Einstein.model.json;
using LibraryFunctionReplacements;
using phi.other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model
{
    public class BaseSynapse : DynamicHoldable
    {
        public const int STRENGTH_MAX_DECIMALS = 4;

        private BaseNeuron _from;
        private BaseNeuron _to;
        private float _strength;
        public BaseNeuron From {
            get { return _from; }
            set {
                _from = validateFrom(value);
                FlagChange();
            }
        }
        public BaseNeuron To
        {
            get { return _to; }
            set {
                _to = validateTo(value);
                FlagChange();
            }
        }

        public float Strength {
            get { return _strength; }
            set {
                _strength = validateStrength(value);
                FlagChange();
            }
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
            return From.ToString() + " --(x" + getStrengthAsStringForUI() + ")--> " + To.ToString();
        }

        public virtual string GetSave() { throw new NotSupportedException(); }

        private BaseNeuron validateFrom(BaseNeuron value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            return value;
        }
        private BaseNeuron validateTo(BaseNeuron value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            return value;
        }
        private float validateStrength(float value)
        {
            value = Math.Max(BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MIN,
                Math.Min(value,
                BibiteConfigVersionIndependent.SYNAPSE_STRENGTH_MAX));
            return value;
        }
        public string getStrengthAsStringForUI()
        {
            return CustomNumberParser.FloatToString(Strength, int.MaxValue, STRENGTH_MAX_DECIMALS);
        }
        public void setStrengthAsStringForUI(string strength)
        {
            Strength = CustomNumberParser.StringToFloat(strength);
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
