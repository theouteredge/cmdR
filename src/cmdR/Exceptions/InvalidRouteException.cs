using System;

namespace cmdR.Exceptions
{
    public class InvalidRouteException : Exception
    {
        public InvalidRouteException(string message) : base(message)
        {
        }
    }
}
