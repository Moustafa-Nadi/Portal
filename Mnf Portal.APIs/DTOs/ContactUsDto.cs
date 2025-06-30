using Mnf_Portal.Core.Enums;

namespace Mnf_Portal.APIs.DTOs
{
    public class ContactUsDto
    {
        public string Email { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public MessageType Type { get; set; }
    }
}
