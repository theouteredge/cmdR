using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message)
        {
        }
    }
}
