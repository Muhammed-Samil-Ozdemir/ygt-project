namespace Ygt.WebAPI.Dtos;

/// <summary>IsVisible boş gelirse kişi görünür sayılır. Fotoğraf ayrı uçtan yüklenir.</summary>
public sealed record TeamMemberCommandDto(
    string? Name,
    string? Role,
    bool? IsVisible);
