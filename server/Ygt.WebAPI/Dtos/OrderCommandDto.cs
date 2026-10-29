namespace Ygt.WebAPI.Dtos;

/// <summary>Listenin yeni sırası: kayıt ID'leri ilk sıradan son sıraya.</summary>
public sealed record OrderCommandDto(List<Guid>? Ids);
