using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Api.Exceptions;
using Jil;
using Polly;

namespace Api
{
    public class ApiClient
    {
        private const string TOKEN_ENDPOINT = "/Token";
        private const string CLIENT_ID = "EvotoClient";

        private static readonly Options JilOptions = new Options(dateFormat: DateTimeFormat.ISO8601);

        protected static readonly CurrentAuthentication CurrentAuth = new CurrentAuthentication();

        private readonly HttpClient _client;

        private readonly RefreshTokenProvider _refreshTokenProvider;
        private readonly Policy _retryPolicy;

        public ApiClient(string baseUri)
        {
#if DEBUG
            const string url = "http://localhost:15893";
#else
            const string url = "http://api.evoto.tech";
#endif
            _retryPolicy = Policy
                .Handle<UnauthorizedException>()
                .RetryAsync(3, async (exception, retryCount) =>
                        await RefreshTokensAsync());

            var handler = new WebRequestHandler();
#if DEBUG
            handler.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
#endif
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{url}/{baseUri}")
            };

            _refreshTokenProvider = new RefreshTokenProvider(TOKEN_ENDPOINT, _client, CLIENT_ID);
        }

        /// <summary>
        ///     Sets the Authorization header to use bearer authorization and the
        ///     supplied token. This applies to all subsequent requests.
        /// </summary>
        private void SetAuthorizationHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentAuth.AuthenticationToken);
        }

        /// <summary>
        ///     Clears the Authorization header, to ensure we are working with a clean start.
        /// </summary>
        private void ClearAuthorizationHeader()
        {
            _client.DefaultRequestHeaders.Clear();
        }

        /// <summary>
        ///     Makes a HTTP GET call to a specified endpoint and deserializes the
        ///     response to a specified type.
        /// </summary>
        /// <typeparam name="T">An object the expected response can be deserialized to.</typeparam>
        /// <param name="uri">The uri to make the GET request to.</param>
        /// <param name="args">Uri format args.</param>
        /// <returns>Returns an object of type <c>T</c>.</returns>
        public async Task<T> GetAsync<T>(string uri, params object[] args)
            where T : class
        {
            HttpResponseMessage response = null;

            if (CurrentAuth.Expired)
                await RefreshTokensAsync();

            await _retryPolicy.ExecuteAsync(async () =>
            {
                uri = string.Format(uri, args);
                response = await _client.GetAsync(uri);

                if (IsUnauthorized(response))
                    throw new UnauthorizedException();
                if (!response.IsSuccessStatusCode)
                    throw new ApiErrorException();
            });

            if (response == null)
                throw new ApiErrorException();

            var json = await response.Content.ReadAsStringAsync();
            return JSON.Deserialize<T>(json, JilOptions);
        }


        /// <summary>
        ///     Makes a HTTP POST call to a specified endpoint, serializing any
        ///     response to a dynamic object.
        /// </summary>
        /// <param name="uri">The uri to make the POST request to.</param>
        /// <param name="args">Uri format args.</param>
        /// <param name="body">An object that will be serialized as JSON before being sent as the request body.</param>
        /// <returns>Returns a <c>dynamic</c> object.</returns>
        public Task<dynamic> PostAsync(string uri, object body, params object[] args)
        {
            return PostAsync<dynamic>(uri, body, args);
        }

        /// <summary>
        ///     Makes a HTTP POST call to a specified endpoint, serializing any
        ///     response to a dynamic object.
        /// </summary>
        /// <param name="uri">The uri to make the POST request to.</param>
        /// <param name="args">Uri format args.</param>
        /// <param name="body">An object that will be serialized as JSON before being sent as the request body.</param>
        /// <returns>Returns a <c>T</c> object.</returns>
        public async Task<T> PostAsync<T>(string uri, object body, params object[] args) where T : class
        {
            HttpResponseMessage response = null;
            var content = new StringContent(JSON.SerializeDynamic(body));
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            if (CurrentAuth.Expired)
                await RefreshTokensAsync();

            await _retryPolicy.ExecuteAsync(async () =>
            {
                uri = string.Format(uri, args);
                response = await _client.PostAsync(uri, content);

                if (IsUnauthorized(response))
                    throw new UnauthorizedException();
                if (!response.IsSuccessStatusCode)
                    throw new ApiErrorException();
            });

            if (response == null)
                throw new ApiErrorException();

            var json = await response.Content.ReadAsStringAsync();
            return json.Length == 0 ? null : JSON.Deserialize<T>(json, JilOptions);
        }

        /// <summary>
        ///     Makes a HTTP DELETE call to a specified endpoint, serializing any
        ///     response to a dynamic object.
        /// </summary>
        /// <param name="uri">The uri to make the POST request to.</param>
        /// <param name="args">Uri format args.</param>
        /// <returns>Returns a <c>T</c> object.</returns>
        public async Task DeleteAsync(string uri, params object[] args)
        {
            HttpResponseMessage response;

            if (CurrentAuth.Expired)
                await RefreshTokensAsync();

            await _retryPolicy.ExecuteAsync(async () =>
            {
                uri = string.Format(uri, args);
                response = await _client.DeleteAsync(uri);

                if (IsUnauthorized(response))
                    throw new UnauthorizedException();
                if (!response.IsSuccessStatusCode)
                    throw new ApiErrorException();
            });
        }

        /// <summary>
        ///     Logs in the user using username and password.
        ///     Stores the token for subsequent requests throughout the system.
        ///     Clears any previous login data.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        public async Task LoginAsync(string username, string password)
        {
            //Clear existing auth
            ClearAuthorizationHeader();

            var data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", CLIENT_ID},
                {"grant_type", "password"},
                {"username", username},
                {"password", password}
            });
            var response = await _client.PostAsync(TOKEN_ENDPOINT, data);
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new IncorrectLoginException(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
                throw new UnableToObtainTokenException();

            Debug.WriteLine("Logged in successfully");

            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var update = JSON.Deserialize<TokenUpdate>(content, JilOptions);
                CurrentAuth.Update(update);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not read Token Response. {e.Message}");
            }

            SetAuthorizationHeader();
        }

        // Protected so that the token returned from registering can be pushed back through
        protected void UpdateAuth(TokenUpdate update)
        {
            CurrentAuth.Update(update);
        }

        /// <summary>
        ///     Refreshes the current auth tokens.
        /// </summary>
        private async Task RefreshTokensAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentAuth.RefreshToken))
                throw new UnauthorizedException("No refresh token available.");

            var response = await _refreshTokenProvider.RefreshTokenAsync(CurrentAuth.RefreshToken);

            CurrentAuth.Update(response);

            SetAuthorizationHeader();
        }

        /// <summary>
        ///     Determines if the supplied <see cref="HttpResponseMessage" />
        ///     indicates an unauthorized request was made.
        /// </summary>
        /// <param name="response">The response to evaluate.</param>
        /// <returns>
        ///     Returns <c>true</c> if the response indicates an unauthorized
        ///     request, false otherwise.
        /// </returns>
        private static bool IsUnauthorized(HttpResponseMessage response)
        {
            return response.StatusCode.Equals(HttpStatusCode.Unauthorized);
        }
    }
}