namespace OpenWorld.Server.Authentication
{
    public class AuthenticationError
    {
        public AuthenticationErrorReason Reason { get; }
        public string UserErrorMessage { get; }

        public AuthenticationError(AuthenticationErrorReason reason, string userErrorMessage)
        {
            Reason = reason;
            UserErrorMessage = userErrorMessage;
        }
    }
}