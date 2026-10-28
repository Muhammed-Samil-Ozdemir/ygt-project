using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Standart etkinlik formuna yapılan bireysel başvuru. Eski sistemdeki "formAppeals" tablosu.</summary>
public sealed class FormApplication : BaseEntity
{
    public Guid FormId { get; set; }
    public Form Form { get; set; } = default!;

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = default!;
}
