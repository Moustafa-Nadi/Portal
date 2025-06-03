using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Infrastructure.Identity;
using Mnf_Portal.Infrastructure.Identity.IdentityDataSeed;
using Mnf_Portal.Infrastructure.Persistence;

namespace Mnf_Portal.APIs.Extensions
{
    public static class MigrationManager
    {
        public static async Task<WebApplication> ApplyMigrationsAndSeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope(); // Creates a new dependency injection scope.

            var services = scope.ServiceProvider;

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = services.GetRequiredService<MnfDbContext>();
                await dbContext.Database.MigrateAsync();  // Apply migrations at startup

                await MnfDbContextSeed.SeedingAsync(dbContext);

                var identityContext = services.GetRequiredService<MnfIdentityDbContext>(); // Ask CLR For Creating Object From AppIdentityDbContext Explicitly

                await identityContext.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await AppIdentityDbContextSeed.SeedUsersAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An Error Occurs When Applying The Migrations");
            }
            return app;
        }
    }
}
