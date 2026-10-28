using Ygt.WebAPI.Abstractions;
using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Models;

/// <summary>Etkinlik başvuru formu. Aynı anda en fazla bir form aktif olabilir.</summary>
public sealed class Form : BaseEntity
{
    public string Title { get; set; } = default!;

    /// <summary>Kısa açıklama, zorunlu değil.</summary>
    public string? Content { get; set; }

    /// <summary>Eski sistemde başlıkta "Hackathon" geçip geçmediğine bakılıyordu.</summary>
    public FormType Type { get; set; } = FormType.Standard;

    public bool IsActive { get; set; } = true;

    public List<FormApplication> Applications { get; set; } = [];
    public List<HackathonApplication> HackathonApplications { get; set; } = [];
}
