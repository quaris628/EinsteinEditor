using Bibyte.functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibyte.functional.brains
{
    public class BrainToGenerate
    {
        // This brain will be generated when the bibyte program is run.
        // You should change it to one of your brains.
        public static readonly IFunctionalProgrammingBrain
            BRAIN_TO_GENERATE = new TestingBrain0_5();

        private BrainToGenerate() { }
    }
}
