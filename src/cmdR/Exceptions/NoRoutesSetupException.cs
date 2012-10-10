using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class NoRoutesSetupException : Exception
    {
        public NoRoutesSetupException(string message) : base(message)
        {
        }
    }
}
