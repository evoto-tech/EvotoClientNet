using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    [Serializable]
    public class ApiErrorException : ApiException
    {
        public ApiErrorException()
        {
        }

        public ApiErrorException(string message) : base(message)
        {
        }

        public ApiErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ApiErrorException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}