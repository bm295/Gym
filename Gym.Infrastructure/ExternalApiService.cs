using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Gym.Application;

namespace Gym.Infrastructure
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<string> GetExchangeTokenAsync(
            string tokenExchangeEndpoint,
            string subjectToken,
            string audience,
            string scope,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tokenExchangeEndpoint))
                throw new ArgumentException("Token exchange endpoint is required.", nameof(tokenExchangeEndpoint));

            if (string.IsNullOrWhiteSpace(subjectToken))
                throw new ArgumentException("Subject token is required.", nameof(subjectToken));

            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentException("Audience is required.", nameof(audience));

            var client = _httpClientFactory.CreateClient();
            var parameters = new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:token-exchange",
                ["subject_token"] = subjectToken,
                ["subject_token_type"] = "urn:ietf:params:oauth:token-type:access_token",
                ["audience"] = audience,
                ["scope"] = scope ?? string.Empty
            };

            using var content = new FormUrlEncodedContent(parameters);
            using var response = await client.PostAsync(tokenExchangeEndpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenExchangeResponse>(cancellationToken: cancellationToken);
            return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Exchange token response did not contain an access token.");
        }

        public async Task<string> CallExternalApiAsync(
            string apiUrl,
            string exchangeToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException("API URL is required.", nameof(apiUrl));

            if (string.IsNullOrWhiteSpace(exchangeToken))
                throw new ArgumentException("Exchange token is required.", nameof(exchangeToken));

            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", exchangeToken);

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        private sealed class TokenExchangeResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
