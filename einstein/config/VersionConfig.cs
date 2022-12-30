using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein
{
    public struct VersionConfig
    {
        public const int INPUT_NODES_INDEX_MIN = 0;
        public const int INPUT_NODES_INDEX_MAX = 32;
        public const int OUTPUT_NODES_INDEX_MIN = 33;
        public const int OUTPUT_NODES_INDEX_MAX = 47;
        public const int HIDDEN_NODES_INDEX_MIN = 48;
        public const int HIDDEN_NODES_INDEX_MAX = int.MaxValue;
    }
}
