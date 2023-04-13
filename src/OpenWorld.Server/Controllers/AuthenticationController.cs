using Microsoft.AspNetCore.Mvc;
using OpenWorld.Models.Authentication;
using OpenWorld.Server.Authentication;
using OpenWorld.Server.Authentication.Models;

namespace OpenWorld.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLogin userLogin)
        {
            var result = await _authenticationService.AuthenticateAsync(userLogin);

            if (!result.IsSuccessful)
            {
                return BadRequest(new LoginErrorResponse(result.Error!.UserErrorMessage));
            }

            var token = _authenticationService.GenerateToken(result.Success!.User);

            return Ok(new LoginSuccessResponse(token));
        }
    }
}
