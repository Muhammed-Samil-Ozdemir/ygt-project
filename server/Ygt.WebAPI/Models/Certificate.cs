using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Doğrulanabilir sertifika.</summary>
public sealed class Certificate : BaseEntity
{
    /// <summary>
    /// Doğrulama linkinde kullanılan kod (eski sistemdeki sertifika ID'si).
    /// Basılı sertifikalardaki linkler bozulmasın diye eski değerler aynen korunur.
    /// Eski kodların bir kısmı geçerli bir GUID olmadığı için metin olarak tutulur.
    /// </summary>
    public string Code { get; set; } = default!;

    public string CourseName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
