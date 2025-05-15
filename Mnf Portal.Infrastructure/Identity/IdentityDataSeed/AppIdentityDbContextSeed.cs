using Microsoft.AspNetCore.Identity;
using Mnf_Portal.Core.Entities.Identity;

namespace Mnf_Portal.Infrastructure.Identity.IdentityDataSeed
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    FirstName = "Mustafa",
                    LastName = "Nadi",
                    Email = "Mustafa.Nadi@linkdev.com",
                    UserName = "Mustafa.Nadi",
                    PhoneNumber = "0101452681"
                };
                await userManager.CreateAsync(user, "P$$0W0rdz124");
            }
        }
    }
}
