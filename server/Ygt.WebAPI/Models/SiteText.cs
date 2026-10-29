using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>
/// Sitedeki tek parça metin (giriş başlığı, alt bilgi, KVKK metni...) ya da görsel (logo, sekme simgesi...).
/// Anahtar/değer olarak saklanır.
/// Hangi anahtarların olduğu, etiketleri ve sınırları SiteTextCatalog'da tanımlı.
/// </summary>
public sealed class SiteText : BaseEntity
{
    public string Key { get; set; } = default!;

    /// <summary>
    /// Düz metin; HTML olarak yorumlanmaz. Görselde dosya adıdır. Boşsa sitede o alan gösterilmez
    /// (ya da katalogdaki genel ifade yazılır).
    /// </summary>
    public string Value { get; set; } = "";
}
