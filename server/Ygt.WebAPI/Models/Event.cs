using Ygt.WebAPI.Abstractions;
using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Models;

/// <summary>
/// Ana sayfada fotoğraflarıyla gösterilen etkinlik (DevFest, Coffee Talk...). Sadece tanıtım içindir;
/// başvuru alınan etkinlik formlarından (Form) ayrıdır.
/// </summary>
public sealed class Event : BaseEntity, ISortable
{
    public string Title { get; set; } = default!;

    /// <summary>Kısa açıklama, zorunlu değil.</summary>
    public string? Description { get; set; }

    public EventStyle Style { get; set; } = EventStyle.Wall;

    public int SortOrder { get; set; }

    /// <summary>Gizlenen etkinlik sitede görünmez, panelde kalır.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>SortOrder'a göre ilki kapak fotoğrafıdır. Fotoğrafı olmayan etkinlik sitede gösterilmez.</summary>
    public List<EventPhoto> Photos { get; set; } = [];
}
