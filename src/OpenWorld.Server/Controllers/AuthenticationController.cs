using Microsoft.AspNetCore.Mvc;
using OpenWorld.Models.Authentication;
using OpenWorld.Server.Authentication;
using OpenWorld.Server.Authentication.Models;
using OpenWorld.Server.Users;

namespace OpenWorld.Server.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(IAuthenticationService authenticationService, IUserService userService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IUserService _userService = userService;

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

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegistration userRegistration)
    {
        if (string.IsNullOrWhiteSpace(userRegistration.Username))
        {
            return BadRequest(new RegisterErrorResponse("Username is missing."));
        }

        if (string.IsNullOrWhiteSpace(userRegistration.Password))
        {
            return BadRequest(new RegisterErrorResponse("Password is missing."));
        }

        var result = await _userService.AddUserAsync(userRegistration.Username, userRegistration.Password);

        if (!result.IsSuccessful)
        {
            return BadRequest(new RegisterErrorResponse(result.Error!.UserErrorMessage));
        }

        // TODO: include url to fetch user details
        return Created();
    }
}
