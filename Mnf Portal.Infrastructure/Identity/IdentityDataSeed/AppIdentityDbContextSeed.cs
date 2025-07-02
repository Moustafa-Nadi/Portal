using Microsoft.AspNetCore.Identity;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.OtherObjects;

namespace Mnf_Portal.Infrastructure.Identity.IdentityDataSeed
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            bool isOwnerExists = await roleManager.RoleExistsAsync("OWNER"),
             isAdminExists = await roleManager.RoleExistsAsync("ADMIN"),
             isUserExists = await roleManager.RoleExistsAsync("USER");

            if (!isOwnerExists && !isAdminExists && !isUserExists)
            {
                await roleManager.CreateAsync(new IdentityRole("OWNER"));
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
                await roleManager.CreateAsync(new IdentityRole("USER"));
            }

            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    FirstName = "Mustafa",
                    LastName = "Nadi",
                    Email = "Admin@gmail.com",
                    UserName = "Mustafa.Nadi",
                    PhoneNumber = "0101452681"
                };
                await userManager.CreateAsync(user, "P$$0W0rdz124");
                await userManager.AddToRoleAsync(user, StaticUserRoles.USER);
                await userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
                await userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);
            }
        }
    }
}
