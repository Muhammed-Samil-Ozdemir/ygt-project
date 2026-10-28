using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class VisitorCounterConfiguration : IEntityTypeConfiguration<VisitorCounter>
{
    public void Configure(EntityTypeBuilder<VisitorCounter> builder)
    {
        builder.ToTable("VisitorCounters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Page)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Page)
            .IsUnique();
    }
}
