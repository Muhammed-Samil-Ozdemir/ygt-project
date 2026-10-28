using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>
/// Hackathon formuna yapılan takım başvurusu. Eski sistemdeki "hackathonForms" tablosu.
/// Takım 2 veya 3 kişi; 3. üyenin alanları boş olabilir.
/// </summary>
public sealed class HackathonApplication : BaseEntity
{
    public Guid FormId { get; set; }
    public Form Form { get; set; } = default!;

    public string TeamName { get; set; } = default!;

    public string Member1Name { get; set; } = default!;
    public string Member1Phone { get; set; } = default!;
    public string Member1Email { get; set; } = default!;

    public string Member2Name { get; set; } = default!;
    public string Member2Phone { get; set; } = default!;
    public string Member2Email { get; set; } = default!;

    public string? Member3Name { get; set; }
    public string? Member3Phone { get; set; }
    public string? Member3Email { get; set; }
}
