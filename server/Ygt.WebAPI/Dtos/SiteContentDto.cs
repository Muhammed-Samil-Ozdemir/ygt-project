namespace Ygt.WebAPI.Dtos;

/// <summary>
/// Sitenin bütün içeriği; site açılışta tek istekte alır. Sadece görünür kayıtlar, panelde verilen sırayla gelir.
/// Texts anahtarları SiteTextCatalog'daki anahtarlardır, ör. "heroTitle".
/// </summary>
public sealed record SiteContentDto(
    Dictionary<string, string> Texts,
    List<AboutCardDto> About,
    List<TeamMemberDto> Team,
    List<string> Technologies,
    List<SiteLinkDto> Links);
