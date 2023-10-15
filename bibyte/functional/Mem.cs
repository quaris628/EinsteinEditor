using Bibyte.functional.background;
using Bibyte.functional.background.booleans;
using Bibyte.functional.background.values;
using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional
{
    public class Mem
    {

        public static Number Latch(Number val)
        {
            return new HiddenNeuronNum(val, NeuronType.Latch);
        }
        public static Bool Store(Bool shouldStore, Bool toStore)
        {
            return new StoredBool(shouldStore, toStore);
        }
        public static Bool Store(Bool shouldStore, Bool toStore, bool initialValue)
        {
            return new StoredBool(shouldStore, toStore, initialValue);
        }
        public static Number Store(Bool shouldStore, Number toStore)
        {
            return new StoredNum(shouldStore, toStore);
        }
        public static Number Store(Bool shouldStore, Number toStore, float initialValue)
        {
            return new StoredNum(shouldStore, toStore, initialValue);
        }
        public static Bool ToggleOf(Bool boolean)
        {
            return new ToggledBool(boolean, false);
        }
        public static Bool ToggleOf(Bool boolean, bool initialValue)
        {
            return new ToggledBool(boolean, initialValue);
        }
    }
}
