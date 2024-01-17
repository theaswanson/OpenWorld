using OpenWorld.Server.Authentication.Models;
using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public interface IUserService
{
    Task<RegistrationResult> AddUserAsync(string username, string password);
    Task<User?> GetUserAsync(string username);
    Task RemoveUserAsync(string username);
}
