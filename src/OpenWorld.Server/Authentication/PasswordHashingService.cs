using System.Security.Cryptography;
using System.Text;

namespace OpenWorld.Server.Authentication
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private const string PasswordHashingKey = "KEY1";

        public async Task<string> HashPasswordAsync(string password)
        {
            var hashAlgorithm = new HMACSHA512(Encoding.ASCII.GetBytes(PasswordHashingKey));

            var hashBytes = await hashAlgorithm.ComputeHashAsync(new MemoryStream(Encoding.ASCII.GetBytes(password)));

            return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
        }
    }
}
