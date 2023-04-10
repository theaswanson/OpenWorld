using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OpenWorld.Server.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Dictionary<string, User> _users = new()
        {
            { "admin", new User("admin", "79b890890374fcc41cd8643eeb8095f54b57607f271a43dcd6f43aaf4e445a164c47b116bde0b5f782b968cb15d734154da7eb0bac3fecc837cdf50f402b8a1c", "Administrator") }
        };

        private const string PasswordHashingKey = "KEY1";
        private const string SigningKey = "KEY2KEY2KEY2KEY2";

        private readonly Dictionary<AuthenticationErrorReason, string> _errorMessages = new()
        {
            { AuthenticationErrorReason.Unknown, "Unknown failure." },
            { AuthenticationErrorReason.InvalidUsername, "Username is missing or invalid." },
            { AuthenticationErrorReason.InvalidPassword, "Password is missing or invalid." },
            { AuthenticationErrorReason.UserNotFound, "Incorrect username or password." },
            { AuthenticationErrorReason.IncorrectPassword, "Incorrect username or password." },
        };

        public async Task<AuthenticationResult> AuthenticateAsync(UserLogin userLogin)
        {
            if (string.IsNullOrWhiteSpace(userLogin.Username))
            {
                return ErrorResult(AuthenticationErrorReason.InvalidUsername);
            }

            if (string.IsNullOrWhiteSpace(userLogin.Password))
            {
                return ErrorResult(AuthenticationErrorReason.InvalidPassword);
            }

            if (!_users.ContainsKey(userLogin.Username))
            {
                return ErrorResult(AuthenticationErrorReason.UserNotFound);
            }

            var user = _users[userLogin.Username];

            string hashedPassword = await GetHashedPassword(userLogin.Password);

            if (hashedPassword != user.PasswordHash)
            {
                return ErrorResult(AuthenticationErrorReason.IncorrectPassword);
            }

            return new AuthenticationResult(new AuthenticationSuccess(user));

            static async Task<string> GetHashedPassword(string password)
            {
                var hashAlgorithm = new HMACSHA512(Encoding.ASCII.GetBytes(PasswordHashingKey));

                var hashBytes = await hashAlgorithm.ComputeHashAsync(new MemoryStream(Encoding.ASCII.GetBytes(password)));

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: "TEST_ISSUER",
                audience: "TEST_AUDIENCE",
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private AuthenticationResult ErrorResult(AuthenticationErrorReason authenticationErrorReason)
        {
            var userErrorMessage = GetUserErrorMessage(authenticationErrorReason);

            return new AuthenticationResult(new AuthenticationError(authenticationErrorReason, userErrorMessage));

            string GetUserErrorMessage(AuthenticationErrorReason authenticationErrorReason)
            {
                if (!_errorMessages.ContainsKey(authenticationErrorReason))
                {
                    return "Unknown failure.";
                }

                return _errorMessages[authenticationErrorReason];
            }
        }
    }
}
