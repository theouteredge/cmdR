using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class InvalidRouteException : Exception
    {
        public InvalidRouteException(string message) : base(message)
        {
        }
    }
}
