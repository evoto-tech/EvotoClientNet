using System;
using System.Net;

namespace Api
{
    internal class ApiException : Exception
    {
        public ApiException(HttpStatusCode code, string err) : base($"API Error ({code}) - {err}")
        {
        }
    }
}