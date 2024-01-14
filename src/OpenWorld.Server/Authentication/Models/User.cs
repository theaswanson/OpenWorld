namespace OpenWorld.Server.Authentication.Models;

public record User(string Username, string PasswordHash, string Role);