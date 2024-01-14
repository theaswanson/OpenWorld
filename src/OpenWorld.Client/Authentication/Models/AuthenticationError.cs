namespace OpenWorld.Client.Authentication.Models;

public record AuthenticationError(AuthenticationErrorReason Reason, string Message);
