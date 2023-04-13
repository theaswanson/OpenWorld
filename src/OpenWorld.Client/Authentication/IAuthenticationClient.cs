namespace OpenWorld.Client.Authentication
{
    internal interface IAuthenticationClient
    {
        Task<bool> AuthenticateAsync(string username, string password);
    }
}
