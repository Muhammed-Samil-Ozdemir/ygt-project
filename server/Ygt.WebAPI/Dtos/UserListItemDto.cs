namespace Ygt.WebAPI.Dtos;

/// <summary>IsCurrent: listeyi isteyen yöneticinin kendisi mi (kendini silemez).</summary>
public sealed record UserListItemDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    bool IsCurrent);
