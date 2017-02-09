using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    public abstract class ApiException : Exception
    {
        protected ApiException()
        {
        }

        protected ApiException(string msg) : base(msg)
        {
        }

        protected ApiException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}