namespace OpenWorld.Server.Authentication.Models;

public record RegistrationError(RegistrationErrorReason Reason, string UserErrorMessage);
