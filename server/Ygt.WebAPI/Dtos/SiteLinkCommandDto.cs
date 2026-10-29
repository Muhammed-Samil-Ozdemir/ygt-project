namespace Ygt.WebAPI.Dtos;

/// <summary>IsVisible boş gelirse bağlantı görünür sayılır.</summary>
public sealed record SiteLinkCommandDto(
    string? Label,
    string? Url,
    bool? IsVisible);
