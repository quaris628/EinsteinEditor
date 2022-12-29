using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein
{
    public struct VersionConfig
    {
        public const int OUTPUT_NODES_INDEX_MIN = 33;
        public const int OUTPUT_NODES_INDEX_MAX = 47;

        public const int INPUT_NODES_INDEX_MIN = 0;
        public const int INPUT_NODES_INDEX_MAX = OUTPUT_NODES_INDEX_MIN - 1;
        public const int HIDDEN_NODES_INDEX_MIN = OUTPUT_NODES_INDEX_MAX + 1;
    }
}
