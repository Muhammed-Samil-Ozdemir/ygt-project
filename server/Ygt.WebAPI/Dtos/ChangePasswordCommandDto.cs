namespace Ygt.WebAPI.Dtos;

/// <summary>Yöneticinin kendi şifresini değiştirmesi; mevcut şifre doğrulanır.</summary>
public sealed record ChangePasswordCommandDto(
    string? CurrentPassword,
    string? NewPassword);
