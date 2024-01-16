using OpenWorld.Server.Authentication.Models;
using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public interface IUserService
{
    Task<RegistrationResult> AddUserAsync(string username, string password);
    User? GetUser(string username);
    void RemoveUser(string username);
}
