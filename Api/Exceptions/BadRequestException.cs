using System;
using System.Linq;
using Api.Responses;

namespace Api.Exceptions
{
    [Serializable]
    public class BadRequestException : ApiException
    {
        public BadRequestException(ModelStateResponse modelState)
            : base(GetMessage(modelState))
        {
        }

        private static string GetMessage(ModelStateResponse ms)
        {
            if (ms.ModelState == null)
                return ms.Message;
            return $"{string.Join("\n", ms.ModelState.SelectMany(kv => kv.Value)).Replace(". ", "\n")}";
        }
    }
}