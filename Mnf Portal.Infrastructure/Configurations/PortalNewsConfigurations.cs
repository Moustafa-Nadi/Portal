using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnf_Portal.Core.Entities;

namespace Mnf_Portal.Infrastructure.Configurations
{
    internal class PortalNewsConfigurations : IEntityTypeConfiguration<PortalNews>
    {
        public void Configure(EntityTypeBuilder<PortalNews> builder)
        {
            builder.ToTable("prtl_News");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.IsFeatured)
                .HasDefaultValue(false);

            builder.Property(n => n.Date)
                   .HasColumnName("News_date")
                   .IsRequired();

            builder.Property(n => n.Image)
                   .HasColumnName("News_img");

            builder.Property(n => n.OwnerId)
                    .HasColumnName("Owner_ID")
                   .IsRequired();

            builder.HasMany(n => n.Translations)
                   .WithOne(t => t.News)
                   .HasForeignKey(t => t.NewsId);
        }
    }
}
