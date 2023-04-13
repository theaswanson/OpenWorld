using System.Text.Json;

namespace OpenWorld.Client
{
    internal class OpenWorldHttpClient : IOpenWorldHttpClient
    {
        private readonly JsonSerializerOptions _deserializerOptions = new()
        {
            // Required to work with .NET Pascal casing
            PropertyNameCaseInsensitive = true
        };

        public async Task<HttpResult<TSuccess, TError>> PostAsync<TSuccess, TError>(string url, HttpContent body)
        {
            var httpClient = new HttpClient();

            var httpResponse = await httpClient.PostAsync(url, body);

            var responseStream = await httpResponse.Content.ReadAsStreamAsync();

            return !httpResponse.IsSuccessStatusCode
                ? new HttpResult<TSuccess, TError>(await DeserializeAsync<TError>(responseStream), httpResponse.StatusCode)
                : new HttpResult<TSuccess, TError>(await DeserializeAsync<TSuccess>(responseStream), httpResponse.StatusCode);
        }

        private async Task<T> DeserializeAsync<T>(Stream stream)
        {
            var value = await JsonSerializer.DeserializeAsync<T>(stream, _deserializerOptions);

            return value is null
                ? throw new Exception($"Failed to deserialize to type {typeof(T).FullName}")
                : value;
        }
    }
}
