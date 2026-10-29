namespace Ygt.WebAPI.Dtos;

public sealed record CurrentUserDto(
    Guid Id,
    string Name,
    string Email);
