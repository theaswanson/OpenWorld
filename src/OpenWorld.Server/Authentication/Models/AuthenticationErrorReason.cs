namespace OpenWorld.Server.Authentication.Models;

public enum AuthenticationErrorReason
{
    Unknown,
    InvalidUsername,
    InvalidPassword,
    UserNotFound,
    IncorrectPassword
}
