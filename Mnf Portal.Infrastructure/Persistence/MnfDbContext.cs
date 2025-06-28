using Microsoft.EntityFrameworkCore;
using Mnf_Portal.Core.Entities;
using System.Reflection;

namespace Mnf_Portal.Infrastructure.Persistence
{
    public class MnfDbContext(DbContextOptions<MnfDbContext> options) : DbContext(options)
    {
        public DbSet<PortalNews> News { get; set; }
        public DbSet<NewsTranslation> Translations { get; set; }
        public DbSet<NewsGallary> Gallaries { get; set; }
        public DbSet<ContactUs> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
