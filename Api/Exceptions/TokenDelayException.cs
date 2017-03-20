using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    [Serializable]
    public class TokenDelayException : ApiException
    {
        public TokenDelayException()
        {
        }

        public TokenDelayException(string message) : base(message)
        {
        }

        public TokenDelayException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TokenDelayException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}