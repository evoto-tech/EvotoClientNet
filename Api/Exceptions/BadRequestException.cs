using System;
using System.Linq;
using Api.Responses;

namespace Api.Exceptions
{
    [Serializable]
    public class BadRequestException : ApiException
    {
        public BadRequestException(ModelStateResponse modelState)
            : base($"{string.Join("\n", modelState.ModelState.SelectMany(kv => kv.Value))}")
        {
        }
    }
}