using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Eğitim (kurs).</summary>
public sealed class Course : BaseEntity
{
    public string Title { get; set; } = default!;

    /// <summary>Kısa açıklama, zorunlu değil.</summary>
    public string? Content { get; set; }

    /// <summary>Kapatılan kurslar silinmez, pasife çekilir.</summary>
    public bool IsActive { get; set; } = true;

    public List<CourseApplication> Applications { get; set; } = [];
}
