using System;
using System.Runtime.Serialization;

namespace Api
{
    [Serializable]
    public class UnableToObtainTokenException : Exception
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