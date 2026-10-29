namespace Ygt.WebAPI.Dtos;

public sealed record SiteLinkListItemDto(
    Guid Id,
    string Label,
    string Url,
    bool IsVisible);
