using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Ana sayfadaki "Hakkımızda" bölümünün kartları ("Biz kimiz?", "Neler yaparız?"...).</summary>
public sealed class AboutCard : BaseEntity, ISortable
{
    public string Title { get; set; } = default!;
    public string Text { get; set; } = default!;

    public int SortOrder { get; set; }

    /// <summary>Gizlenen kart sitede görünmez, panelde kalır.</summary>
    public bool IsVisible { get; set; } = true;
}
