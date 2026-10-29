using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Style boş gelirse Wall (etkinlik panosu), IsVisible boş gelirse görünür sayılır.</summary>
public sealed record EventCommandDto(
    string? Title,
    string? Description,
    EventStyle? Style,
    bool? IsVisible);
