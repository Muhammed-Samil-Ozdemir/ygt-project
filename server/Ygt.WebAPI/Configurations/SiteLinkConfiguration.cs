using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class SiteLinkConfiguration : IEntityTypeConfiguration<SiteLink>
{
    public void Configure(EntityTypeBuilder<SiteLink> builder)
    {
        builder.ToTable("SiteLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);
    }
}
