using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.Infrastructure.Configurations
{
    public class NewsGallaryConfiguration : IEntityTypeConfiguration<NewsGallary>
    {
        public void Configure(EntityTypeBuilder<NewsGallary> builder) => builder.HasOne(i => i.PortalNews)
            .WithMany(n => n.Gallaries)
            .HasForeignKey(i => i.NewsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
