using OpenWorld.Server.Authentication.Models;

namespace OpenWorld.Server.Authentication
{
    public interface IUserService
    {
        Task AddUser(string username, string password);
        User? GetUser(string username);
        void RemoveUser(string username);
    }
}