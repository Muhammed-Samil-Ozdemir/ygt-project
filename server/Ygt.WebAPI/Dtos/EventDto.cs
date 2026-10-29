using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Dtos;

/// <summary>Photos: fotoğrafların dosya adları, ilki kapak; /site/content/images/{dosya} adresinden alınır.</summary>
public sealed record EventDto(string Title, string? Description, EventStyle Style, List<string> Photos);
