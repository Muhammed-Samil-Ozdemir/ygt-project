using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Alt bilgideki "Bizi takip edin" bağlantısı (Instagram, LinkedIn...).</summary>
public sealed class SiteLink : BaseEntity, ISortable
{
    public string Label { get; set; } = default!;
    public string Url { get; set; } = default!;

    public int SortOrder { get; set; }

    /// <summary>Gizlenen bağlantı sitede görünmez, panelde kalır.</summary>
    public bool IsVisible { get; set; } = true;
}
