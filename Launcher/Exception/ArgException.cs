using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Ex
{
    public abstract class ArgException : Exception
    {
        internal ArgException(string message) : base(message) { }
    }

    public class TooManyArgException : ArgException
    {
        public TooManyArgException() : base(Properties.ExResources.ArgTooMany) { }
    }
}
