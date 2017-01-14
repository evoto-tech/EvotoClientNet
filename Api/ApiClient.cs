using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace Api
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(string controller)
        {
            var apiBase = ConfigurationManager.AppSettings["apiBase"];
            _client = new RestClient($"{apiBase}/{controller}");

#if DEBUG
            // Ignore self-signed certs in DEBUG
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
#endif
        }

        public async Task Exec(string uri, Method method, object data)
        {
            await Exec<dynamic>(uri, method, data);
        }

        public async Task<T> Exec<T>(string uri, Method method, object data)
        {
            var req = new RestRequest(uri, method);
            req.AddHeader("cache-control", "no-cache");
            req.AddHeader("content-type", "application/json");

            if (data != null)
                req.AddParameter("application/json", data, ParameterType.RequestBody);

            var res = await _client.ExecuteTaskAsync<T>(req);
            if (IsSuccessCode(res.StatusCode))
                return res.Data;

            var e = new ApiException(res.StatusCode, req.Method.ToString(), res.ResponseUri.ToString(), res.ErrorMessage);
            Debug.WriteLine(e.Message);
            throw e;
        }

        private static bool IsSuccessCode(HttpStatusCode statusCode)
        {
            return ((int) statusCode >= 200) && ((int) statusCode <= 299);
        }
    }
}