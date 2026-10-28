using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class CourseApplicationConfiguration : IEntityTypeConfiguration<CourseApplication>
{
    public void Configure(EntityTypeBuilder<CourseApplication> builder)
    {
        builder.ToTable("CourseApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TcNumber)
            .HasMaxLength(11);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(32);

        // Aynı e-postayla aynı kursa ikinci başvuru olmaz. Çift tıklamada iki istek
        // aynı anda gelse bile ikinci kaydı veritabanı reddeder.
        builder.HasIndex(x => new { x.CourseId, x.Email })
            .IsUnique();
    }
}
