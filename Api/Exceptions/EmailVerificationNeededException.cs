using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    [Serializable]
    public class EmailVerificationNeededException : ApiException
    {
        public EmailVerificationNeededException()
        {
        }

        public EmailVerificationNeededException(string message) : base(message)
        {
        }

        public EmailVerificationNeededException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EmailVerificationNeededException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}