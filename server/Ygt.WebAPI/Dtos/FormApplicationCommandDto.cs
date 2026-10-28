namespace Ygt.WebAPI.Dtos;

public sealed record FormApplicationCommandDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber);
