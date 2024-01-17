using OpenWorld.Data.Users.Models;

namespace OpenWorld.Data.Users;
public interface IUserRepository
{
    Task<(bool Successful, int UserId)> CreateAsync(string username, string passwordHash, string role);
    Task<bool> DeleteAsync(string username);
    Task<(bool Found, User? User)> GetAsync(string username);
}
