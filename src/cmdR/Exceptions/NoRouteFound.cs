using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class NoRouteFound : Exception
    {
        public NoRouteFound(string message) : base(message)
        {
        }
    }
}
