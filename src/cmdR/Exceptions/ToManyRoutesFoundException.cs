using System;

namespace cmdR.Exceptions
{
    public class ToManyRoutesFoundException : Exception
    {
        public ToManyRoutesFoundException(string message) : base(message)
        {
        }
    }
}
