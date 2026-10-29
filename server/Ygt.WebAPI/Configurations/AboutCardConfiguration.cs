using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class AboutCardConfiguration : IEntityTypeConfiguration<AboutCard>
{
    public void Configure(EntityTypeBuilder<AboutCard> builder)
    {
        builder.ToTable("AboutCards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
