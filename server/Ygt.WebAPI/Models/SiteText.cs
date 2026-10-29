using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>
/// Sitedeki tek parça metin (giriş başlığı, alt bilgi, KVKK metni...). Anahtar/değer olarak saklanır.
/// Hangi anahtarların olduğu, etiketleri ve sınırları SiteTextCatalog'da tanımlı.
/// </summary>
public sealed class SiteText : BaseEntity
{
    public string Key { get; set; } = default!;

    /// <summary>Düz metin; HTML olarak yorumlanmaz. İsteğe bağlı metin boşsa sitede o alan gösterilmez.</summary>
    public string Value { get; set; } = "";
}
