namespace Ygt.WebAPI.Dtos;

public sealed record CertificateCommandDto(
    string? CourseName,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber);
