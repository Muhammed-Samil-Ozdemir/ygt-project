using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Sayfa ziyaret sayacı. Eski sistemdeki "counters" tablosu.</summary>
public sealed class VisitorCounter : BaseEntity
{
    public const string HomePage = "home";

    public string Page { get; set; } = default!;
    public int Count { get; set; }
}
