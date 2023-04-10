using System.Text;
using System.Text.Json;

namespace OpenWorld.Client
{
    internal class AuthenticationClient : IAuthenticationClient
    {
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(
                "https://localhost:7192/auth/login",
                new StringContent(
                    JsonSerializer.Serialize(new { Username = username, Password = password }),
                    Encoding.UTF8,
                    "application/json")
                );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
