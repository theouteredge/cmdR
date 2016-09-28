using System;

namespace cmdR.Exceptions
{
    public class NoRouteFoundException : Exception
    {
        public NoRouteFoundException(string message) : base(message)
        {
        }
    }
}
