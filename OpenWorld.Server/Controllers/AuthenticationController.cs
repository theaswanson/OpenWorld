using Microsoft.AspNetCore.Mvc;
using OpenWorld.Server.Authentication;

namespace OpenWorld.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        private readonly Dictionary<AuthenticationErrorReason, string> _errorMessages = new()
        {
            { AuthenticationErrorReason.Unknown, "Unknown failure." },
            { AuthenticationErrorReason.InvalidUsername, "Username is missing or invalid." },
            { AuthenticationErrorReason.InvalidPassword, "Password is missing or invalid." },
            { AuthenticationErrorReason.UserNotFound, "Invalid username or password." },
            { AuthenticationErrorReason.IncorrectPassword, "Invalid username or password." },
        };

        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLogin userLogin)
        {
            var result = await _authenticationService.AuthenticateAsync(userLogin);

            if (!result.IsSuccessful)
            {
                var errorMessage = GetErrorMessage(result.Error.Reason);

                _logger.LogInformation("Authentication failure: {errorReason}", result.Error.Reason);

                return BadRequest(errorMessage);
            }

            var token = _authenticationService.GenerateToken(result.Success!.User);

            return Ok(token);

            string GetErrorMessage(AuthenticationErrorReason authenticationErrorReason)
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
