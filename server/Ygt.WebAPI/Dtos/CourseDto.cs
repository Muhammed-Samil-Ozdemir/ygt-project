namespace Ygt.WebAPI.Dtos;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string? Content);
