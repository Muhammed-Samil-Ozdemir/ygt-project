namespace Ygt.WebAPI.Dtos;

/// <summary>Doğrulama sayfasında gösterilen bilgiler. Kişisel iletişim bilgileri yer almaz.</summary>
public sealed record CertificateDto(
    string Code,
    string CourseName,
    string FirstName,
    string LastName,
    bool HasImage);
