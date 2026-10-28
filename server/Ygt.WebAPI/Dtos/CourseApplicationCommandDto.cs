namespace Ygt.WebAPI.Dtos;

public sealed record CourseApplicationCommandDto(
    string? FirstName,
    string? LastName,
    string? TcNumber,
    string? Email,
    string? PhoneNumber,
    DateOnly? BirthDate);
