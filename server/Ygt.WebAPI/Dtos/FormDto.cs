using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

public sealed record FormDto(
    Guid Id,
    string Title,
    string? Content,
    FormType Type);
