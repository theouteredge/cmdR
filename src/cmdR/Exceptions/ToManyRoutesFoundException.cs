using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Exceptions
{
    public class ToManyRoutesFoundException : Exception
    {
        public ToManyRoutesFoundException(string message) : base(message)
        {
        }
    }
}
