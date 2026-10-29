namespace Ygt.WebAPI.Enums;

/// <summary>Site metninin türü. Panel alanı buna göre çizer, API buna göre doğrular.</summary>
public enum SiteTextKind
{
    /// <summary>Tek satır metin.</summary>
    Text,

    /// <summary>Çok satırlı metin; satır sonları sitede korunur.</summary>
    LongText,

    /// <summary>http(s) ya da mailto bağlantısı.</summary>
    Url
}
