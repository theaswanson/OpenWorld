using Microsoft.AspNetCore.Mvc;
using OpenWorld.Server.Authentication;
using OpenWorld.Server.Authentication.Models;

namespace OpenWorld.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

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
                _logger.LogInformation("Authentication failure for user '{user}': {errorReason}", userLogin.Username, result.Error!.Reason);

                return BadRequest(result.Error.UserErrorMessage);
            }

            var token = _authenticationService.GenerateToken(result.Success!.User);

            _logger.LogInformation("Authentication success for user '{user}'.", userLogin.Username);

            return Ok(token);
        }
    }
}
