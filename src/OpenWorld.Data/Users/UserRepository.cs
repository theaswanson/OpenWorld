using Dapper;
using OpenWorld.Data.Users.Models;

namespace OpenWorld.Data.Users;

public class UserRepository : SqliteBaseRepository, IUserRepository
{
    public async Task<(bool Found, User? User)> GetAsync(string username)
    {
        using var conn = SimpleDbConnection();

        await conn.OpenAsync();

        var user = await conn.QuerySingleOrDefaultAsync<User>(
            @"
                SELECT Id, Username, PasswordHash, Role
                FROM User
                WHERE Username = @username", new { username });

        return (user is not null, user);
    }

    public async Task<(bool Successful, int UserId)> CreateAsync(string username, string passwordHash, string role)
    {
        using var conn = SimpleDbConnection();

        await conn.OpenAsync();

        var userId = await conn.QuerySingleAsync<int>(
            @"
                INSERT INTO User (Username, PasswordHash, Role)
                VALUES (@username, @passwordHash, @role);
                SELECT last_insert_rowid()", new { username, passwordHash, role });

        return (userId > 0, userId);
    }

    public async Task<bool> DeleteAsync(string username)
    {
        using var conn = SimpleDbConnection();

        await conn.OpenAsync();

        var rowsAffected = await conn.ExecuteAsync(
            @"
                DELETE FROM User
                WHERE Username = @username", new { username });

        return rowsAffected > 0;
    }
}
