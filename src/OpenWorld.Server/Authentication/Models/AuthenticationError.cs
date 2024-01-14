namespace OpenWorld.Server.Authentication.Models;

public record AuthenticationError(AuthenticationErrorReason Reason, string UserErrorMessage);