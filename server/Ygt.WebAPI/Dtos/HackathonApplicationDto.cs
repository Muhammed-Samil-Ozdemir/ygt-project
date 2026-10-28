namespace Ygt.WebAPI.Dtos;

public sealed record HackathonApplicationDto(
    Guid Id,
    string TeamName,
    HackathonMemberDto Member1,
    HackathonMemberDto Member2,
    HackathonMemberDto? Member3,
    DateTime CreatedAt);
