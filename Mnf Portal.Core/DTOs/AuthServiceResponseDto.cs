using System.Text.Json.Serialization;

namespace Mnf_Portal.Core.DTOs
{
    public class AuthServiceResponseDto
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefreshToken { get; set; }
    }
}
