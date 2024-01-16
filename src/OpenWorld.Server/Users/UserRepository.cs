using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public class UserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users = new()
    {
        { "admin", new User("admin", "79b890890374fcc41cd8643eeb8095f54b57607f271a43dcd6f43aaf4e445a164c47b116bde0b5f782b968cb15d734154da7eb0bac3fecc837cdf50f402b8a1c", "Administrator") }
    };

    public (bool Found, User? user) Get(string username)
    {
        var found = _users.TryGetValue(username, out User? user);

        return (found, user);
    }

    public bool Create(string username, string hashedPassword)
    {
        return _users.TryAdd(username, new User(username, hashedPassword, "User"));
    }

    public bool Delete(string username)
    {
        return _users.Remove(username);
    }
}
