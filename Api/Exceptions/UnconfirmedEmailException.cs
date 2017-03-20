using System;
using System.Runtime.Serialization;

namespace Api.Exceptions
{
    [Serializable]
    public class UnconfirmedEmailException : ApiException
    {
        public UnconfirmedEmailException()
        {
        }

        public UnconfirmedEmailException(string message) : base(message)
        {
        }

        public UnconfirmedEmailException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnconfirmedEmailException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}