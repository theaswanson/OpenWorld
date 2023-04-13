using System.Net;

namespace OpenWorld.Client
{
    public class HttpResult<TSuccess, TError>
    {
        public bool IsSuccessful => Error is null;
        public TSuccess? Success { get; set; }
        public TError? Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public HttpResult(TSuccess success, HttpStatusCode statusCode)
        {
            Success = success;
            StatusCode = statusCode;
        }

        public HttpResult(TError error, HttpStatusCode statusCode)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}
