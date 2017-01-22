using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    public class IncorrectLoginException : Exception
    {
        public IncorrectLoginException()
        {
        }

        public IncorrectLoginException(string message) : base(message)
        {
        }

        public IncorrectLoginException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IncorrectLoginException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}