using Bibyte.functional;
using Bibyte.functional.booleans;
using Bibyte.functional.memory;
using Bibyte.functional.values;
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

        public static Value Latch(Value val)
        {
            return new HiddenNeuronVal(val, NeuronType.Latch);
        }
        public static Bool Store(Bool shouldStore, Bool toStore)
        {
            return new StoredBool(shouldStore, toStore);
        }
        public static Bool Store(Bool shouldStore, Bool toStore, bool initialValue)
        {
            return new StoredBool(shouldStore, toStore, initialValue);
        }
        public static Value Store(Bool shouldStore, Value toStore)
        {
            return new StoredValue(shouldStore, toStore);
        }
        public static Value Store(Bool shouldStore, Value toStore, float initialValue)
        {
            return new StoredValue(shouldStore, toStore, initialValue);
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
