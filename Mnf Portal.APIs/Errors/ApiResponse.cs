using System.Net;

namespace Mnf_Portal.APIs.Errors
{
    public class ApiResponse(int statusCode, string? message = null)
    {
        public int StatusCode { get; set; } = statusCode;

        public string? Message
        {
            get;
            set;
        } = message ??
            statusCode switch
            {
                400 => nameof(HttpStatusCode.BadRequest),
                401 => nameof(HttpStatusCode.Unauthorized),
                403 => nameof(HttpStatusCode.Forbidden),
                404 => nameof(HttpStatusCode.NotFound),
                500 => nameof(HttpStatusCode.InternalServerError),
                _ => null
            };
    }
}
