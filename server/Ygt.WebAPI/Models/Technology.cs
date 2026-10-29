using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Ana sayfadaki "Çalıştığımız teknolojiler" etiketi.</summary>
public sealed class Technology : BaseEntity, ISortable
{
    public string Name { get; set; } = default!;

    public int SortOrder { get; set; }
}
