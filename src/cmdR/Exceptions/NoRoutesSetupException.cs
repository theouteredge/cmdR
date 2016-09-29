using System;

namespace cmdR.Exceptions
{
    public class NoRoutesSetupException : Exception
    {
        public NoRoutesSetupException(string message) : base(message)
        {
        }
    }
}
