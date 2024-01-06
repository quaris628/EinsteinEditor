using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config.bibiteVersions
{
    public class CannotConvertException : Exception
    {
        public CannotConvertException() : base() { }
        public CannotConvertException(string message) : base(message) { }
        public CannotConvertException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
