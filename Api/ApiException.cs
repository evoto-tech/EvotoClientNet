using System;
using System.Net;

namespace Api
{
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode code, string method, string uri, string err)
            : base($"API Error {method}: {uri} ({code}) - {err}")
        {
            StatusCode = code;
        }

        public HttpStatusCode StatusCode { get; }
    }
}