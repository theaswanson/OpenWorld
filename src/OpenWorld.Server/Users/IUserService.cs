using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public interface IUserService
{
    Task AddUser(string username, string password);
    User? GetUser(string username);
    void RemoveUser(string username);
}