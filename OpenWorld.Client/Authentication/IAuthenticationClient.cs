namespace OpenWorld.Client.Authentication
{
    internal interface IAuthenticationClient
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}