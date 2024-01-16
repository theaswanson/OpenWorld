using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;
public interface IUserRepository
{
    bool Create(string username, string hashedPassword);
    bool Delete(string username);
    (bool Found, User? user) Get(string username);
}