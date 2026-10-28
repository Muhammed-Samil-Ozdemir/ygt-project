namespace Ygt.WebAPI.Dtos;

/// <summary>3. üye isteğe bağlı: hiç gönderilmeyebilir ya da alanları boş bırakılabilir.</summary>
public sealed record HackathonApplicationCommandDto(
    string? TeamName,
    HackathonMemberDto? Member1,
    HackathonMemberDto? Member2,
    HackathonMemberDto? Member3);
