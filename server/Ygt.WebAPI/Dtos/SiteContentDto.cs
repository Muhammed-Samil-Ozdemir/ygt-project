namespace Ygt.WebAPI.Dtos;

/// <summary>
/// Sitenin bütün içeriği; site açılışta tek istekte alır. Sadece görünür kayıtlar, panelde verilen sırayla gelir.
/// Texts anahtarları SiteTextCatalog'daki anahtarlardır, ör. "heroTitle". Boş metnin yerine varsa genel ifadesi
/// (Placeholder) gelir; görsellerde değer dosya adıdır, /site/content/images/{dosya} adresinden alınır.
/// Fotoğrafı olmayan etkinlikler gelmez.
/// </summary>
public sealed record SiteContentDto(
    Dictionary<string, string> Texts,
    List<AboutCardDto> About,
    List<EventDto> Events,
    List<TeamMemberDto> Team,
    List<string> Technologies,
    List<SiteLinkDto> Links);
