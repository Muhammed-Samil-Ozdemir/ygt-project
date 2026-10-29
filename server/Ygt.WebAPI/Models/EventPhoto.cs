using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Etkinliğin fotoğrafı. Dosya, sitenin diğer görselleriyle aynı klasörde durur.</summary>
public sealed class EventPhoto : BaseEntity, ISortable
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = default!;

    /// <summary>Yüklenen dosyanın adı; her yüklemede yeni ad verilir.</summary>
    public string FileName { get; set; } = default!;

    public int SortOrder { get; set; }
}
