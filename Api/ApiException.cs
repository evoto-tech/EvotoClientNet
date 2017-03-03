using System.Net;
using Api.Exceptions;

namespace Api
{
    public class ApiStatusException : ApiException
    {
        public ApiStatusException(HttpStatusCode code, string method, string uri, string err)
            : base($"API Error {method}: {uri} ({code}) - {err}")
        {
            StatusCode = code;
        }

        public HttpStatusCode StatusCode { get; }
    }
}