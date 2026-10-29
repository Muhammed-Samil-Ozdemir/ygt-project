namespace Ygt.WebAPI.Dtos;

/// <summary>Photo: fotoğrafın dosya adı, /site/content/photos/{photo} adresinden alınır. Fotoğraf yoksa boş.</summary>
public sealed record TeamMemberDto(string Name, string Role, string? Photo);
