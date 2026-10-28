namespace Ygt.WebAPI.Dtos;

public sealed record CourseApplicationDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? TcNumber,
    string Email,
    string? PhoneNumber,
    DateOnly? BirthDate,
    DateTime CreatedAt);
