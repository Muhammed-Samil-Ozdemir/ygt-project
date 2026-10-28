namespace Ygt.WebAPI.Dtos;

public sealed record FileDto(byte[] Content, string ContentType, string FileName);
