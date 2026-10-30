using Ygt.WebAPI.Context;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Tests;

/// <summary>
/// Başlangıç içeriği ve örnek veri panelin kurallarına uymalı; aksi halde panelde ilk kaydetmede hata çıkar.
/// Görsellerin derleme çıktısına kopyalandığı da burada kontrol edilir. Veritabanı gerektirmez.
/// </summary>
public sealed class SeedContentTests
{
    private static readonly string SeedDataPath = Path.Combine(AppContext.BaseDirectory, "SeedData");

    [Fact]
    public void Default_texts_follow_catalog_rules()
    {
        foreach (var (key, value) in DefaultContent.Texts)
        {
            var definition = SiteTextCatalog.Find(key);
            Assert.True(definition is not null, $"Katalogda olmayan anahtar: {key}");
            Assert.NotEqual(SiteTextKind.Image, definition.Kind);

            var label = $"{definition.Label} ({definition.Group})";
            var cleaned = definition.Kind == SiteTextKind.Url
                ? Input.OptionalUrl(value, label)
                : Input.OptionalText(value, label, definition.MaxLength);

            Assert.Equal(value, cleaned);
        }
    }

    [Fact]
    public void Default_images_exist()
    {
        foreach (var (key, file) in DefaultContent.Images)
        {
            Assert.Equal(SiteTextKind.Image, SiteTextCatalog.Find(key)?.Kind);
            Assert.True(File.Exists(Path.Combine(SeedDataPath, "default", file)), $"Görsel yok: {file}");
        }
    }

    [Fact]
    public void Sample_photos_exist()
    {
        foreach (var member in SampleContent.Team)
            Assert.True(File.Exists(Path.Combine(SeedDataPath, "sample", "team", member.Photo)), $"Fotoğraf yok: {member.Photo}");

        foreach (var item in SampleContent.Events)
        {
            var directory = Path.Combine(SeedDataPath, "sample", "events", item.Folder);
            Assert.True(Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0, $"Fotoğraf yok: {item.Folder}");
        }
    }

    [Fact]
    public void Sample_people_are_valid()
    {
        foreach (var person in SampleContent.People)
        {
            Assert.Equal(person.Phone, Input.RequiredPhone(person.Phone, "Telefon"));
            Assert.Equal(person.Email, Input.RequiredEmail(person.Email, "E-posta"));
        }
    }
}
