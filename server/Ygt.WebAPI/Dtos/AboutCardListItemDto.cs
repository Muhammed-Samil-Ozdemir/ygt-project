namespace Ygt.WebAPI.Dtos;

public sealed record AboutCardListItemDto(
    Guid Id,
    string Title,
    string Text,
    string? Image,
    bool IsVisible);
