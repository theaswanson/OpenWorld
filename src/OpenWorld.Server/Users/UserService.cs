using OpenWorld.Server.Authentication;
using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public class UserService(
    IPasswordHashingService passwordHashingService,
    ILogger<UserService> logger) : IUserService
{
    private readonly IPasswordHashingService _passwordHashingService = passwordHashingService;
    private readonly ILogger<UserService> _logger = logger;

    private readonly Dictionary<string, User> _users = new()
    {
        { "admin", new User("admin", "79b890890374fcc41cd8643eeb8095f54b57607f271a43dcd6f43aaf4e445a164c47b116bde0b5f782b968cb15d734154da7eb0bac3fecc837cdf50f402b8a1c", "Administrator") }
    };

    public User? GetUser(string username)
    {
        var successfullyFound = _users.TryGetValue(username, out var user);

        if (!successfullyFound)
        {
            _logger.LogDebug("Failed to get user with username '{user}'.", username);

            return null;
        }

        _logger.LogDebug("Fetched user '{user}'.", username);

        return user;
    }

    public async Task AddUser(string username, string password)
    {
        if (_users.ContainsKey(username))
        {
            throw new Exception($"User already exists with username '{username}'.");
        }

        var hashedPassword = await _passwordHashingService.HashPasswordAsync(password);

        var successfullyAdded = _users.TryAdd(username, new User(username, hashedPassword, "User"));

        if (successfullyAdded)
        {
            _logger.LogInformation("Added user '{user}'.", username);
        }
        else
        {
            _logger.LogError("Failed to add user with username '{user}'.", username);
        }
    }

    public void RemoveUser(string username)
    {
        if (!_users.ContainsKey(username))
        {
            throw new Exception($"User does not exist with username '{username}'.");
        }

        var successfullyRemoved = _users.Remove(username);

        if (successfullyRemoved)
        {
            _logger.LogInformation("Removed user '{user}'.", username);
        }
        else
        {
            _logger.LogError("Failed to remove user '{user}'.", username);
        }
    }
}
