using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    [Serializable]
    public class UnableToObtainTokenException : ApiException
    {
        public UnableToObtainTokenException()
        {
        }

        public UnableToObtainTokenException(string message) : base(message)
        {
        }

        public UnableToObtainTokenException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnableToObtainTokenException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}