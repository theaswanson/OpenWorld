using OpenWorld.Client.Authentication.Models;
using OpenWorld.Models.Authentication;
using System.Text;
using System.Text.Json;

namespace OpenWorld.Client.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IOpenWorldHttpClient _httpClient;

        public AuthenticationService(IOpenWorldHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            var httpResponse = await _httpClient.PostAsync<LoginSuccessResponse, LoginErrorResponse>(
                "https://localhost:7192/auth/login",
                new StringContent(
                    JsonSerializer.Serialize(new { Username = username, Password = password }),
                    Encoding.UTF8,
                    "application/json")
                );

            return httpResponse.IsSuccessful
                ? SuccessResult(httpResponse)
                : ErrorResult(httpResponse);

            static AuthenticationResult ErrorResult(
                HttpResult<LoginSuccessResponse, LoginErrorResponse> httpResponse)
            {
                if (((int)httpResponse.StatusCode) >= 500)
                {
                    return new AuthenticationResult(
                        new AuthenticationError(
                            AuthenticationErrorReason.ServerError,
                            "Server unavailable."));
                }

                return new AuthenticationResult(
                    new AuthenticationError(
                        AuthenticationErrorReason.InvalidCredentials,
                        httpResponse.Error!.Error));
            }

            static AuthenticationResult SuccessResult(
                HttpResult<LoginSuccessResponse, LoginErrorResponse> httpResponse)
            {
                return new AuthenticationResult(new AuthenticationSuccess(httpResponse.Success!.Token));
            }
        }
    }
}
