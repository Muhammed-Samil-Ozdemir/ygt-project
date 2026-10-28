namespace Ygt.WebAPI.Dtos;

public sealed record FormApplicationDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string PhoneNumber,
    DateTime CreatedAt);
