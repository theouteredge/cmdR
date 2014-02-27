using System;

namespace cmdR.Exceptions
{
    public class InvalidMethodSignatureException : Exception
    {
        public InvalidMethodSignatureException(string message) : base(message)
        {
        }
    }
}