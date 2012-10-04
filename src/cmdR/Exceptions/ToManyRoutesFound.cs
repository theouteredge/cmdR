using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class ToManyRoutesFound : Exception
    {
        public ToManyRoutesFound(string message) : base(message)
        {
        }
    }
}
