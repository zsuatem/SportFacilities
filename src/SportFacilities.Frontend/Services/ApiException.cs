using System.Net;

namespace SportFacilities.Frontend.Services
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string Response { get; }

        public ApiException(HttpStatusCode statusCode, string response)
            : base($"API Error: {statusCode}")
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}
