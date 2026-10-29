namespace Ygt.WebAPI.Dtos;

/// <summary>FileName: /site/content/images/{fileName} adresinden alınır. Id, fotoğrafı silmek ve sıralamak için.</summary>
public sealed record EventPhotoDto(Guid Id, string FileName);
