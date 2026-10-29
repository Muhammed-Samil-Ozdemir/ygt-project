namespace Ygt.WebAPI.Dtos;

public sealed record UserUpdateCommandDto(
    string? Name,
    string? Email);
