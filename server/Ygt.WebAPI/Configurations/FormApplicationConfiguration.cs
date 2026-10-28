using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class FormApplicationConfiguration : IEntityTypeConfiguration<FormApplication>
{
    public void Configure(EntityTypeBuilder<FormApplication> builder)
    {
        builder.ToTable("FormApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(32);

        // Aynı telefonla aynı forma ikinci başvuru olmaz (telefon zorunlu, e-posta değil).
        builder.HasIndex(x => new { x.FormId, x.PhoneNumber })
            .IsUnique();
    }
}
