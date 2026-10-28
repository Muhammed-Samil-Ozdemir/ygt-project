using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Geri sayım ayarlanmamışsa Countdown boş gelir.</summary>
public sealed record SiteSettingsDto(HomeButton HomeButton, CountdownDto? Countdown);
