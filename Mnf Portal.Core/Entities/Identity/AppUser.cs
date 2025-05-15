using Microsoft.AspNetCore.Identity;

namespace Mnf_Portal.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
