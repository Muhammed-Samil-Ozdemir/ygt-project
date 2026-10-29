using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Configurations;

public sealed class SiteTextConfiguration : IEntityTypeConfiguration<SiteText>
{
    public void Configure(EntityTypeBuilder<SiteText> builder)
    {
        builder.ToTable("SiteTexts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(50);

        // Uzunluk sınırı anahtara göre SiteTextCatalog'da; KVKK metni gibi uzun metinler de burada durur.
        builder.Property(x => x.Value)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();
    }
}
