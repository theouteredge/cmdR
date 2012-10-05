using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class NoRouteFoundException : Exception
    {
        public NoRouteFoundException(string message) : base(message)
        {
        }
    }
}
