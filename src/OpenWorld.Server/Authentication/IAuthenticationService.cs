using OpenWorld.Server.Authentication.Models;
using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(UserLogin userLogin);
    string GenerateToken(User user);
}