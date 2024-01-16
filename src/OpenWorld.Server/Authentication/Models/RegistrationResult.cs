namespace OpenWorld.Server.Authentication.Models;

public class RegistrationResult : Result<RegistrationSuccess, RegistrationError>
{
    public RegistrationResult(RegistrationSuccess success) : base(success)
    {
    }

    public RegistrationResult(RegistrationError error) : base(error)
    {
    }
}
