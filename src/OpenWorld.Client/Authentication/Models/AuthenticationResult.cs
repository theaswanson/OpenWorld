namespace OpenWorld.Client.Authentication.Models;

public class AuthenticationResult
{
    public bool IsSuccessful => Error is null;
    public AuthenticationSuccess? Success { get; }
    public AuthenticationError? Error { get; }

    public AuthenticationResult(AuthenticationSuccess success)
    {
        Success = success;
    }

    public AuthenticationResult(AuthenticationError error)
    {
        Error = error;
    }
}
