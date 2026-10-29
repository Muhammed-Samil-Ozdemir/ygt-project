namespace Ygt.WebAPI.Dtos;

/// <summary>Değişecek metinler, anahtar -> değer. Gönderilmeyen anahtarlar olduğu gibi kalır.</summary>
public sealed record SiteTextsCommandDto(Dictionary<string, string?>? Values);
