using OpenWorld.Client.Authentication.Models;

namespace OpenWorld.Client.Authentication
{
    internal class AuthenticationClient : IAuthenticationClient
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationClient(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var result = await _authenticationService.AuthenticateAsync(username, password);

            if (!result.IsSuccessful)
            {
                await Console.Out.WriteLineAsync(FailureMessage(result.Error!));
                return false;
            }

            // TODO: handle token
            await Console.Out.WriteLineAsync("Logged in.");
            await Console.Out.WriteLineAsync(result.Success!.Token);

            return true;
        }

        private string FailureMessage(AuthenticationError error)
        {
            return error.Reason switch
            {
                AuthenticationErrorReason.Unknown => "Hmm, login failed. Please try again.",
                AuthenticationErrorReason.InvalidCredentials => $"{error.Message} Please try again.",
                AuthenticationErrorReason.ServerError => "Login servers are offline. Please try again later.",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
