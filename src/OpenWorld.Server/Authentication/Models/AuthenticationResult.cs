namespace OpenWorld.Server.Authentication.Models;

public class AuthenticationResult : Result<AuthenticationSuccess, AuthenticationError>
{
    public AuthenticationResult(AuthenticationSuccess success) : base(success)
    {
    }

    public AuthenticationResult(AuthenticationError error) : base(error)
    {
    }
}
