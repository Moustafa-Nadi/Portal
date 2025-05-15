using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Entities.Identity;

namespace Mnf_Portal.Infrastructure.Identity
{
    public class MnfIdentityDbContext : IdentityDbContext<AppUser>
    {
        public MnfIdentityDbContext(DbContextOptions<MnfIdentityDbContext> options) : base(options)
        {

        }
    }
}
