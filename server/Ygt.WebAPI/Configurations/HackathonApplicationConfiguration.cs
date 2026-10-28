using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class HackathonApplicationConfiguration : IEntityTypeConfiguration<HackathonApplication>
{
    public void Configure(EntityTypeBuilder<HackathonApplication> builder)
    {
        builder.ToTable("HackathonApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TeamName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Member1Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Member1Phone).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Member1Email).IsRequired().HasMaxLength(256);

        builder.Property(x => x.Member2Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Member2Phone).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Member2Email).IsRequired().HasMaxLength(256);

        builder.Property(x => x.Member3Name).HasMaxLength(100);
        builder.Property(x => x.Member3Phone).HasMaxLength(32);
        builder.Property(x => x.Member3Email).HasMaxLength(256);

        // Aynı formda takım adı tekrar edemez.
        builder.HasIndex(x => new { x.FormId, x.TeamName })
            .IsUnique();
    }
}
