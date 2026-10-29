using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class EventPhotoConfiguration : IEntityTypeConfiguration<EventPhoto>
{
    public void Configure(EntityTypeBuilder<EventPhoto> builder)
    {
        builder.ToTable("EventPhotos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(100);
    }
}
