namespace OpenWorld.Server.Users.Models;

public record User(string Username, string PasswordHash, string Role);
