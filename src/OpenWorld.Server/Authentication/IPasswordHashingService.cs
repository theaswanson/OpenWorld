namespace OpenWorld.Server.Authentication;

public interface IPasswordHashingService
{
    Task<string> HashPasswordAsync(string password);
}