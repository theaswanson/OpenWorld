namespace OpenWorld.Server.Authentication
{
    public enum AuthenticationErrorReason
    {
        Unknown,
        InvalidUsername,
        InvalidPassword,
        UserNotFound,
        IncorrectPassword
    }
}