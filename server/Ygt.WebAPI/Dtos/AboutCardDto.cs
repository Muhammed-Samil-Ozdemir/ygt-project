namespace Ygt.WebAPI.Dtos;

/// <summary>Image: görselin dosya adı, /site/content/images/{image} adresinden alınır. Görsel yoksa boş.</summary>
public sealed record AboutCardDto(string Title, string Text, string? Image);
