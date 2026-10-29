using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Panelde bir metin alanı: tanımı ve şu anki değeri.</summary>
public sealed record SiteTextDto(
    string Key,
    string Group,
    string Label,
    string? Hint,
    SiteTextKind Kind,
    int MaxLength,
    bool IsRequired,
    string Value);
