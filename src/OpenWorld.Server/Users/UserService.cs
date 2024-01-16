using OpenWorld.Server.Authentication;
using OpenWorld.Server.Users.Models;

namespace OpenWorld.Server.Users;

public class UserService(
    IUserRepository userRepository,
    IPasswordHashingService passwordHashingService,
    ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHashingService _passwordHashingService = passwordHashingService;
    private readonly ILogger<UserService> _logger = logger;

    public User? GetUser(string username)
    {
        var (successfullyFound, user) = _userRepository.Get(username);

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
        if (_userRepository.Get(username).Found)
        {
            throw new Exception($"User already exists with username '{username}'.");
        }

        var hashedPassword = await _passwordHashingService.HashPasswordAsync(password);

        var successfullyAdded = _userRepository.Create(username, hashedPassword);

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
        if (!_userRepository.Get(username).Found)
        {
            throw new Exception($"User does not exist with username '{username}'.");
        }

        var successfullyRemoved = _userRepository.Delete(username);

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
