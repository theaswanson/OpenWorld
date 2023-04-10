namespace OpenWorld.Server.Authentication
{
    public record User(string Username, string PasswordHash, string Role);
}