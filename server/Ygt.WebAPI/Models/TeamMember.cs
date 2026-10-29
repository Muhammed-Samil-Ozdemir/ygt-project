using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Ana sayfadaki "Ekibimiz" bölümünde gösterilen kişi.</summary>
public sealed class TeamMember : BaseEntity, ISortable
{
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;

    /// <summary>Yüklenen fotoğrafın dosya adı. Her yüklemede yeni ad verilir; fotoğraf yoksa boş.</summary>
    public string? PhotoFileName { get; set; }

    public int SortOrder { get; set; }

    /// <summary>Gizlenen kişi sitede görünmez, panelde kalır.</summary>
    public bool IsVisible { get; set; } = true;
}
