using OpenWorld.Client.Authentication.Models;
using System.Text;
using System.Text.Json;

namespace OpenWorld.Client.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(
                "https://localhost:7192/auth/login",
                new StringContent(
                    JsonSerializer.Serialize(new { Username = username, Password = password }),
                    Encoding.UTF8,
                    "application/json")
                );

            if (!response.IsSuccessStatusCode)
            {
                if (((int)response.StatusCode) >= 500)
                {
                    return new AuthenticationResult(new AuthenticationError(AuthenticationErrorReason.ServerError, "Server unavailable."));
                }

                var failureReason = await response.Content.ReadAsStringAsync();

                return new AuthenticationResult(new AuthenticationError(AuthenticationErrorReason.GeneralFailure, failureReason));
            }

            var token = await response.Content.ReadAsStringAsync();

            return new AuthenticationResult(new AuthenticationSuccess(token));
        }
    }
}
