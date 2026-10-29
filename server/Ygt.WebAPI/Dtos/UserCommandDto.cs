namespace Ygt.WebAPI.Dtos;

public sealed record UserCommandDto(
    string? Name,
    string? Email,
    string? Password);
