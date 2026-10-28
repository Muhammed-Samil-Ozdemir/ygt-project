using Ygt.WebAPI.Abstractions;
using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Models;

/// <summary>
/// Ön yüzü yöneten site ayarları. Tabloda tek satır bulunur.
/// Eski sistemdeki "activePanels" ve "activityCounters" tablolarının yerini alır.
/// </summary>
public sealed class SiteSetting : BaseEntity
{
    public HomeButton HomeButton { get; set; } = HomeButton.Form;

    /// <summary>Geri sayım başlığı. Boşsa ön yüz geri sayım göstermez.</summary>
    public string? CountdownTitle { get; set; }

    /// <summary>Geri sayımın bittiği an (UTC).</summary>
    public DateTime? CountdownEndsAt { get; set; }
}
