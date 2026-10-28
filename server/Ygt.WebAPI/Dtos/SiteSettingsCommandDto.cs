using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Geri sayımı kaldırmak için başlık ve tarih boş gönderilir.</summary>
public sealed record SiteSettingsCommandDto(
    HomeButton? HomeButton,
    string? CountdownTitle,
    DateTime? CountdownEndsAt);
