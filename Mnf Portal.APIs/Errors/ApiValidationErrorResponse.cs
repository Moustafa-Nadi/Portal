namespace Mnf_Portal.APIs.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public ApiValidationErrorResponse() : base(400) => Errors = []; // Initialize
    }
}
