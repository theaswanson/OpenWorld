using Dapper;
using Microsoft.Data.Sqlite;

namespace OpenWorld.Data.Users;

public abstract class SqliteBaseRepository
{
    public static async Task InitializeAsync()
    {
        if (!File.Exists(DbFile))
        {
            await CreateDatabaseAsync();
        }

        static async Task CreateDatabaseAsync()
        {
            using var cnn = SimpleDbConnection();

            await cnn.OpenAsync();

            await cnn.ExecuteAsync(
                @"CREATE TABLE User
                (
                    Id              integer primary key AUTOINCREMENT,
                    Username        varchar(32) not null,
                    PasswordHash    varchar(128) not null,
                    Role            varchar(16) not null
                )");

            await cnn.CloseAsync();
        }
    }

    public static SqliteConnection SimpleDbConnection() => new("Data Source=" + DbFile);

    private static string DbFile => Environment.CurrentDirectory + "\\openworld.db";
}

