using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jil;

namespace Api
{
    public class RefreshTokenProvider
    {
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _endpoint;

        public RefreshTokenProvider(string endpoint, HttpClient client, string clientId)
        {
            _endpoint = endpoint;
            _client = client;
            _clientId = clientId;
        }

        public async Task<TokenUpdate> RefreshTokenAsync(string refreshToken)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var body = CreateRefreshBody(refreshToken);
            var response = await _client.PostAsync(_endpoint, body);

            if (response.IsSuccessStatusCode)
                return await DeserializeBody(response.Content);

            throw new UnableToObtainTokenException("Did not receive a success status from the token endpoint.");
        }

        private FormUrlEncodedContent CreateRefreshBody(string refreshToken)
        {
            var info = new Dictionary<string, string>
            {
                {"client_id", _clientId},
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            };

            return new FormUrlEncodedContent(info);
        }

        private static async Task<TokenUpdate> DeserializeBody(HttpContent content)
        {
            try
            {
                var json = await content.ReadAsStringAsync();
                return JSON.Deserialize<TokenUpdate>(json);
            }
            catch (Exception ex)
            {
                throw new UnableToObtainTokenException("Unable to deserialize token endpoint response.", ex);
            }
        }
    }
}