namespace OpenWorld.Client
{
    internal interface IAuthenticationClient
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}