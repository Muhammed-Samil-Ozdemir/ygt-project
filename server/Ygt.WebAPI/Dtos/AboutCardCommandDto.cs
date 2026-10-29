namespace Ygt.WebAPI.Dtos;

/// <summary>IsVisible boş gelirse kart görünür sayılır.</summary>
public sealed record AboutCardCommandDto(
    string? Title,
    string? Text,
    bool? IsVisible);
