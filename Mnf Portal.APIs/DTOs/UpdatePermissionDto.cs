using System.ComponentModel.DataAnnotations;

namespace Mnf_Portal.Core.DTOs
{
    public class UpdatePermissionDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
