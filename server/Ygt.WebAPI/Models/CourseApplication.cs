using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Bir kursa yapılan başvuru. Eski sistemdeki "appeals" tablosu.</summary>
public sealed class CourseApplication : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? TcNumber { get; set; }
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public DateOnly? BirthDate { get; set; }
}
