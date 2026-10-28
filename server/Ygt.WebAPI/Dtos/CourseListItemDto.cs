namespace Ygt.WebAPI.Dtos;

public sealed record CourseListItemDto(
    Guid Id,
    string Title,
    string? Content,
    bool IsActive,
    int ApplicationCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
