using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Entities.Identity;
using System.Reflection;

namespace Mnf_Portal.Infrastructure.Identity
{
    public class MnfIdentityDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public MnfIdentityDbContext(DbContextOptions<MnfIdentityDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.AppUser)
                .WithOne(u => u.RefreshToken)
                .HasForeignKey<RefreshToken>(rt => rt.userId);
        }
    }
}
