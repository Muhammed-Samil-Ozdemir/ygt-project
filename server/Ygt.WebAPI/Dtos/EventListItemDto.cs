using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

public sealed record EventListItemDto(
    Guid Id,
    string Title,
    string? Description,
    EventStyle Style,
    List<EventPhotoDto> Photos,
    bool IsVisible);
