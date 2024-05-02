using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config.bibiteVersions
{
    public class NoSuchVersionException : Exception
    {
        public NoSuchVersionException() : base() { }
        public NoSuchVersionException(string message) : base(message) { }
        public NoSuchVersionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
