using Microsoft.IdentityModel.Tokens;
using OpenWorld.Client.Authentication.Models;

namespace OpenWorld.Client.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    bool IsTokenValid(string token, DateTime now);
    bool IsTokenValid(SecurityToken token, DateTime now);
}
