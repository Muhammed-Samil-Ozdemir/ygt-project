using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>
/// Geri sayım ayarlanmamışsa Countdown boş gelir.
/// ApplicationOpen: ana sayfa butonunun gösterdiği başvuru açık mı (butona göre açık eğitim ya da açık form var mı).
/// Kapalıysa site üst menüde başvuru butonunu göstermez.
/// </summary>
public sealed record SiteSettingsDto(HomeButton HomeButton, CountdownDto? Countdown, bool ApplicationOpen);
