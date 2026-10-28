using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Type boş gelirse Standard sayılır.</summary>
public sealed record FormCommandDto(
    string? Title,
    string? Content,
    FormType? Type);
