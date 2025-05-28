using System.ComponentModel.DataAnnotations;

namespace Mnf_Portal.Core.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one non-alphanumeric character, and be at least 6 characters long.")]
        public string Password { get; set; }

        public string ComfimredPassword { get; set; }
    }
}
