using System;
using System.Diagnostics;
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
            Debug.WriteLine(message);
        }

        public ApiErrorException(string message, Exception inner) : base(message, inner)
        {
            Debug.WriteLine(message);
        }

        protected ApiErrorException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}