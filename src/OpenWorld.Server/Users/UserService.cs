using OpenWorld.Data.Users;
using OpenWorld.Server.Authentication;
using OpenWorld.Server.Authentication.Models;
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

    public async Task<User?> GetUserAsync(string username)
    {
        var (successfullyFound, user) = await _userRepository.GetAsync(username);

        if (!successfullyFound)
        {
            _logger.LogDebug("Failed to get user with username '{user}'.", username);

            return null;
        }

        _logger.LogDebug("Fetched user '{user}'.", username);

        return new User(user!.Username, user.PasswordHash, user.Role);
    }

    public async Task<RegistrationResult> AddUserAsync(string username, string password)
    {
        if (await UserExistsAsync(username))
        {
            return new RegistrationResult(
                new RegistrationError(
                    RegistrationErrorReason.UserAlreadyExists,
                    $"User already exists with username '{username}'."));
        }

        var hashedPassword = await _passwordHashingService.HashPasswordAsync(password);

        var (successful, _) = await _userRepository.CreateAsync(username, hashedPassword, "User");

        if (!successful)
        {
            _logger.LogError("Failed to add user with username '{user}'.", username);

            return new RegistrationResult(
                new RegistrationError(
                    RegistrationErrorReason.Unknown,
                    $"Failed to add user with username '{username}'."));
        }

        _logger.LogInformation("Added user '{user}'.", username);

        return new RegistrationResult(new RegistrationSuccess(username));
    }

    public async Task RemoveUserAsync(string username)
    {
        if (!await UserExistsAsync(username))
        {
            throw new Exception($"User does not exist with username '{username}'.");
        }

        var successfullyRemoved = await _userRepository.DeleteAsync(username);

        if (successfullyRemoved)
        {
            _logger.LogInformation("Removed user '{user}'.", username);
        }
        else
        {
            _logger.LogError("Failed to remove user '{user}'.", username);
        }
    }

    private async Task<bool> UserExistsAsync(string username)
    {
        return (await _userRepository.GetAsync(username)).Found;
    }
}
