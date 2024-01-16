namespace OpenWorld.Server.Authentication.Models;

public abstract class Result<TSuccess, TError>
{
    public bool IsSuccessful => Error is null;
    public TSuccess? Success { get; }
    public TError? Error { get; }

    public Result(TSuccess success) => Success = success;

    public Result(TError error) => Error = error;
}
