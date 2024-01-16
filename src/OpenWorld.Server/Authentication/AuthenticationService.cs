using Microsoft.IdentityModel.Tokens;
using OpenWorld.Server.Authentication.Models;
using OpenWorld.Server.Users;
using OpenWorld.Server.Users.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenWorld.Server.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILogger<AuthenticationService> _logger;

    // TODO: store signing key in configuration
    private const string TokenSigningKey = "KEY2KEY2KEY2KEY2KEY2KEY2KEY2KEY2";

    private readonly Dictionary<AuthenticationErrorReason, string> _errorMessages = new()
    {
        { AuthenticationErrorReason.Unknown, "Unknown failure." },
        { AuthenticationErrorReason.InvalidUsername, "Username is missing or invalid." },
        { AuthenticationErrorReason.InvalidPassword, "Password is missing or invalid." },
        { AuthenticationErrorReason.UserNotFound, "Incorrect username or password." },
        { AuthenticationErrorReason.IncorrectPassword, "Incorrect username or password." },
    };

    public AuthenticationService(
        IUserService userService,
        IPasswordHashingService passwordHashingService,
        ILogger<AuthenticationService> logger)
    {
        _userService = userService;
        _passwordHashingService = passwordHashingService;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(UserLogin userLogin)
    {
        if (string.IsNullOrWhiteSpace(userLogin.Username))
        {
            return Fail(userLogin, AuthenticationErrorReason.InvalidUsername);
        }

        if (string.IsNullOrWhiteSpace(userLogin.Password))
        {
            return Fail(userLogin, AuthenticationErrorReason.InvalidPassword);
        }

        User? user = _userService.GetUser(userLogin.Username);

        if (user is null)
        {
            return Fail(userLogin, AuthenticationErrorReason.UserNotFound);
        }

        string hashedPassword = await _passwordHashingService.HashPasswordAsync(userLogin.Password);

        if (hashedPassword != user.PasswordHash)
        {
            return Fail(userLogin, AuthenticationErrorReason.IncorrectPassword);
        }

        return Success(userLogin, user);
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSigningKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // TODO: store issuer and audience in configuration
        var token = new JwtSecurityToken(
            issuer: "OpenWorld Authority",
            audience: "OpenWorld Client",
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private AuthenticationResult Success(UserLogin userLogin, User user)
    {
        _logger.LogDebug("Authenticated user '{user}'.", userLogin.Username);

        return new AuthenticationResult(new AuthenticationSuccess(user));
    }

    private AuthenticationResult Fail(UserLogin userLogin, AuthenticationErrorReason authenticationErrorReason)
    {
        _logger.LogDebug("Authentication failure for user '{user}': {errorReason}", userLogin.Username, authenticationErrorReason);

        return ErrorResult(authenticationErrorReason);
    }

    private AuthenticationResult ErrorResult(AuthenticationErrorReason authenticationErrorReason)
    {
        var userErrorMessage = GetUserErrorMessage(authenticationErrorReason);

        return new AuthenticationResult(new AuthenticationError(authenticationErrorReason, userErrorMessage));
    }

    private string GetUserErrorMessage(AuthenticationErrorReason authenticationErrorReason)
    {
        if (!_errorMessages.ContainsKey(authenticationErrorReason))
        {
            return "Unknown failure.";
        }

        return _errorMessages[authenticationErrorReason];
    }
}
