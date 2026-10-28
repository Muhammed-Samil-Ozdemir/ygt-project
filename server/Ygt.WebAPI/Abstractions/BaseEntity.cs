namespace Ygt.WebAPI.Abstractions;

/// <summary>
/// Tüm tabloların ortak alanları. CreatedAt ve UpdatedAt, AppDbContext tarafından otomatik doldurulur.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
