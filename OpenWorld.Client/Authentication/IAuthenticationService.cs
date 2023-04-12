using OpenWorld.Client.Authentication.Models;

namespace OpenWorld.Client.Authentication
{
    internal interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    }
}