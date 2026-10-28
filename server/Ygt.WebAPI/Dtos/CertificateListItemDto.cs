namespace Ygt.WebAPI.Dtos;

public sealed record CertificateListItemDto(
    Guid Id,
    string Code,
    string CourseName,
    string FirstName,
    string LastName,
    string? Email,
    string? PhoneNumber,
    bool HasImage,
    DateTime CreatedAt);
