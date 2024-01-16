namespace OpenWorld.Server.Authentication.Models;

public enum RegistrationErrorReason
{
    Unknown,
    BadUsername,
    BadPassword,
    UserAlreadyExists
}
