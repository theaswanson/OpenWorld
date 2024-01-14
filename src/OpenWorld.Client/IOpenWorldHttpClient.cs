namespace OpenWorld.Client;

internal interface IOpenWorldHttpClient
{
    Task<HttpResult<TSuccess, TError>> PostAsync<TSuccess, TError>(string url, HttpContent body);
}