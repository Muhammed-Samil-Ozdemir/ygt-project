using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

public sealed record FormListItemDto(
    Guid Id,
    string Title,
    string? Content,
    FormType Type,
    bool IsActive,
    int ApplicationCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
