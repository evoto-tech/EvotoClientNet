using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace Api
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(string apiUrl)
        {
            _client = new RestClient(apiUrl);

#if DEBUG
            // Ignore self-signed certs in DEBUG
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
#endif
        }

        public async Task Exec(string uri, Method method, string data)
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
            if (!IsSuccessCode(res.StatusCode))
                throw new ApiException(res.StatusCode, res.ErrorMessage);

            return res.Data;
        }

        private static bool IsSuccessCode(HttpStatusCode statusCode)
        {
            return ((int) statusCode >= 200) && ((int) statusCode <= 299);
        }
    }
}