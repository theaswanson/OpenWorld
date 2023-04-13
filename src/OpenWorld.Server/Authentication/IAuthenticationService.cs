using OpenWorld.Server.Authentication.Models;

namespace OpenWorld.Server.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(UserLogin userLogin);
        string GenerateToken(User user);
    }
}