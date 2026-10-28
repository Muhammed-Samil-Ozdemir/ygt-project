using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.ToTable("Forms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Aynı anda en fazla bir aktif form olabilir.
        builder.HasIndex(x => x.IsActive)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        // Formlar silinmez, kapatılır. Yanlışlıkla silinirse başvurular gitmesin diye silme engelli.
        builder.HasMany(x => x.Applications)
            .WithOne(x => x.Form)
            .HasForeignKey(x => x.FormId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.HackathonApplications)
            .WithOne(x => x.Form)
            .HasForeignKey(x => x.FormId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
