using System;
using System.Net;

namespace Api
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public ApiException(HttpStatusCode code, string err) : base($"API Error ({code}) - {err}")
        {
            StatusCode = code;
        }
    }
}