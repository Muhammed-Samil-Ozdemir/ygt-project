using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>
/// Panelde bir metin ya da görsel alanı: tanımı ve kayıtlı değeri. Değer boşsa ""; görselde dosya adıdır.
/// Placeholder: boş bırakılırsa sitede yazılan genel ifade.
/// </summary>
public sealed record SiteTextDto(
    string Key,
    string Group,
    string Label,
    string? Hint,
    SiteTextKind Kind,
    int MaxLength,
    string? Placeholder,
    string Value);
