namespace Ygt.WebAPI.Dtos;

public sealed record TeamMemberListItemDto(
    Guid Id,
    string Name,
    string Role,
    string? Photo,
    bool IsVisible);
