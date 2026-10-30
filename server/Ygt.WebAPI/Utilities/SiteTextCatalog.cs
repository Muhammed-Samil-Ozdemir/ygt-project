using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Utilities;

/// <summary>Panelden değiştirilebilen bir site metninin ya da görselinin tanımı. Panel alanı bu bilgilerle çizer.</summary>
public sealed record SiteTextDefinition
{
    /// <summary>Veritabanındaki ve ön yüzdeki anahtar, ör. "heroTitle".</summary>
    public required string Key { get; init; }

    /// <summary>Panelde alanların gruplandığı başlık.</summary>
    public required string Group { get; init; }

    public required string Label { get; init; }
    public string? Hint { get; init; }
    public SiteTextKind Kind { get; init; } = SiteTextKind.Text;

    /// <summary>Metinlerin en fazla uzunluğu. Görsellerde kullanılmaz.</summary>
    public int MaxLength { get; init; }

    /// <summary>
    /// Metin boş bırakılırsa sitede yazılan genel ifade (ör. bölüm başlığı "Hakkımızda"). Panelde alanın içinde
    /// soluk olarak görünür. Topluluğa özel içerik buraya yazılmaz; o metinler boşken sitede gösterilmez.
    /// </summary>
    public string? Placeholder { get; init; }
}

/// <summary>
/// Sitedeki tek parça metinlerin ve görsellerin (logo, sekme simgesi...) listesi. Bir kısmı ilk kurulumda eski sitenin
/// içeriğiyle gelir (Context/DefaultContent), gerisi boş başlar; hepsi panelden değiştirilir.
/// Yeni alan eklemek için buraya tanım eklemek yeterli; panel alanı kendiliğinden gösterir.
/// Ön yüzde kullanmak için libraries'deki SiteTexts arayüzüne de eklenmeli.
/// </summary>
public static class SiteTextCatalog
{
    private const string General = "Genel";
    private const string Hero = "Ana sayfa girişi";
    private const string Sections = "Bölüm başlıkları";
    private const string Join = "Bize katıl";
    private const string Membership = "Resmi üyelik";
    private const string Games = "Oyunlar";
    private const string Footer = "Alt bilgi";
    private const string Applications = "Başvurular";
    private const string OtherPages = "Diğer sayfalar";

    public static readonly IReadOnlyList<SiteTextDefinition> All =
    [
        new()
        {
            Key = "siteName",
            Group = General,
            Label = "Topluluk adı",
            Hint = "Üst menüde, alt bilgide ve panelde logonun yanında görünür. İlk kelime turuncu yazılır.",
            MaxLength = 100
        },
        new()
        {
            Key = "logo",
            Group = General,
            Label = "Logo",
            Hint = "Üst menüde, alt bilgide, girişte ve geri sayım ekranında görünür. Şeffaf arka planlı PNG önerilir.",
            Kind = SiteTextKind.Image
        },
        new()
        {
            Key = "favicon",
            Group = General,
            Label = "Sekme simgesi (favicon)",
            Hint = "Tarayıcı sekmesinde görünür. Kare bir PNG önerilir. Yüklenmezse logo kullanılır.",
            Kind = SiteTextKind.Image
        },
        new()
        {
            Key = "seoTitle",
            Group = General,
            Label = "Sayfa başlığı",
            Hint = "Tarayıcı sekmesinde ve arama sonuçlarında görünür. Boş bırakılırsa topluluk adı kullanılır.",
            MaxLength = 100
        },
        new()
        {
            Key = "seoDescription",
            Group = General,
            Label = "Sayfa açıklaması",
            Hint = "Arama sonuçlarında başlığın altında görünür.",
            Kind = SiteTextKind.LongText,
            MaxLength = 300
        },
        new()
        {
            Key = "heroEyebrow",
            Group = Hero,
            Label = "Üst başlık",
            Hint = "Büyük başlığın üstündeki küçük yazı.",
            MaxLength = 100
        },
        new()
        {
            Key = "heroTitle",
            Group = Hero,
            Label = "Başlık",
            Hint = "İlk kelime turuncu yazılır. Boş bırakılırsa topluluk adı yazılır.",
            MaxLength = 150
        },
        new()
        {
            Key = "heroLead",
            Group = Hero,
            Label = "Açıklama",
            Kind = SiteTextKind.LongText,
            MaxLength = 500
        },
        new()
        {
            Key = "heroSecondaryButton",
            Group = Hero,
            Label = "İkinci buton",
            Hint = "Hakkımızda bölümüne kaydırır. Boş bırakılırsa gösterilmez.",
            MaxLength = 40
        },
        new()
        {
            Key = "heroImage",
            Group = Hero,
            Label = "Görsel",
            Hint = "Başlığın yanında görünür (dar ekranda gizlenir). Yüklenmezse logo gösterilir.",
            Kind = SiteTextKind.Image
        },
        new()
        {
            Key = "aboutTitle",
            Group = Sections,
            Label = "Hakkımızda",
            Hint = "Bölüm başlığı; üst menüde de bu yazı görünür.",
            MaxLength = 60,
            Placeholder = "Hakkımızda"
        },
        new()
        {
            Key = "eventsTitle",
            Group = Sections,
            Label = "Etkinlikler",
            Hint = "Etkinlik panosunun başlığı; üst menüde de bu yazı görünür.",
            MaxLength = 60,
            Placeholder = "Etkinliklerimiz"
        },
        new()
        {
            Key = "recurringEventsTitle",
            Group = Sections,
            Label = "Geleneksel etkinlikler",
            Hint = "\"Geleneksel\" görünümündeki etkinliklerin bölüm başlığı.",
            MaxLength = 60,
            Placeholder = "Gelenekselleşmiş etkinliklerimiz"
        },
        new()
        {
            Key = "teamTitle",
            Group = Sections,
            Label = "Ekip",
            Hint = "Bölüm başlığı; üst menüde de bu yazı görünür.",
            MaxLength = 60,
            Placeholder = "Ekibimiz"
        },
        new()
        {
            Key = "technologiesTitle",
            Group = Sections,
            Label = "Teknolojiler",
            MaxLength = 60,
            Placeholder = "Çalıştığımız teknolojiler"
        },
        new()
        {
            Key = "linksTitle",
            Group = Sections,
            Label = "Bağlantılar",
            Hint = "Alt bilgideki sosyal medya bağlantılarının başlığı.",
            MaxLength = 60,
            Placeholder = "Bizi takip edin"
        },
        new()
        {
            Key = "joinUrl",
            Group = Join,
            Label = "Katılım formu bağlantısı",
            Hint = "Boş bırakılırsa \"Bize katıl\" butonları ve kartı gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500
        },
        new()
        {
            Key = "joinButton",
            Group = Join,
            Label = "Buton yazısı",
            Hint = "Üst menüde, girişte ve \"Bize katıl\" kartında görünür.",
            MaxLength = 40,
            Placeholder = "Bize katılın"
        },
        new()
        {
            Key = "joinTitle",
            Group = Join,
            Label = "Kart başlığı",
            MaxLength = 100,
            Placeholder = "Bize katıl!"
        },
        new()
        {
            Key = "joinText",
            Group = Join,
            Label = "Kart metni",
            Kind = SiteTextKind.LongText,
            MaxLength = 1000
        },
        new()
        {
            Key = "membershipUrl",
            Group = Membership,
            Label = "Üyelik bağlantısı",
            Hint = "Öğrencilerin resmi üye olduğu sistem, ör. https://topluluk.ktun.edu.tr. Bölüm ana sayfada ekip fotoğraflarının altında görünür; boş bırakılırsa gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500
        },
        new()
        {
            Key = "membershipTitle",
            Group = Membership,
            Label = "Başlık",
            MaxLength = 100,
            Placeholder = "Resmi üye ol"
        },
        new()
        {
            Key = "membershipText",
            Group = Membership,
            Label = "Açıklama",
            Hint = "Ör. üye olma adımları.",
            Kind = SiteTextKind.LongText,
            MaxLength = 500
        },
        new()
        {
            Key = "membershipButton",
            Group = Membership,
            Label = "Buton yazısı",
            MaxLength = 40,
            Placeholder = "Üyelik için tıklayınız"
        },
        new()
        {
            Key = "gamesUrl",
            Group = Games,
            Label = "Oyunların bağlantısı",
            Hint = "Boş bırakılırsa ana sayfadaki bölüm ve üst menüdeki bağlantı gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500
        },
        new()
        {
            Key = "gamesTitle",
            Group = Games,
            Label = "Bölüm başlığı",
            Hint = "Üst menüde de bu yazı görünür.",
            MaxLength = 60,
            Placeholder = "Oyunlar"
        },
        new()
        {
            Key = "gamesText",
            Group = Games,
            Label = "Açıklama",
            Kind = SiteTextKind.LongText,
            MaxLength = 500
        },
        new()
        {
            Key = "gamesButton",
            Group = Games,
            Label = "Buton yazısı",
            MaxLength = 40,
            Placeholder = "Oyunlara git"
        },
        new()
        {
            Key = "gamesImage",
            Group = Games,
            Label = "Görsel",
            Hint = "Kartın yanında görünür, ör. oyunlardan bir ekran görüntüsü. Yüklenmezse oyun kolu simgesi gösterilir.",
            Kind = SiteTextKind.Image
        },
        new()
        {
            Key = "footerText",
            Group = Footer,
            Label = "Tanıtım metni",
            Kind = SiteTextKind.LongText,
            MaxLength = 300
        },
        new()
        {
            Key = "universityName",
            Group = Footer,
            Label = "Üniversite bağlantısının yazısı",
            Hint = "Boş bırakılırsa bağlantının adresi yazılır.",
            MaxLength = 100
        },
        new()
        {
            Key = "universityUrl",
            Group = Footer,
            Label = "Üniversite bağlantısı",
            Hint = "Boş bırakılırsa gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500
        },
        new()
        {
            Key = "courseButton",
            Group = Applications,
            Label = "Eğitim başvurusu butonu",
            Hint = "Üst menüde, ana sayfa butonu eğitime ayarlıyken görünür.",
            MaxLength = 40,
            Placeholder = "Eğitim Başvurusu"
        },
        new()
        {
            Key = "formButton",
            Group = Applications,
            Label = "Etkinlik başvurusu butonu",
            Hint = "Üst menüde, ana sayfa butonu etkinliğe ayarlıyken görünür.",
            MaxLength = 40,
            Placeholder = "Etkinlik Başvurusu"
        },
        new()
        {
            Key = "noCourseText",
            Group = Applications,
            Label = "Açık eğitim yokken",
            Hint = "Başvuruya açık eğitim yokken eğitim başvurusu sayfasında görünür.",
            Kind = SiteTextKind.LongText,
            MaxLength = 300,
            Placeholder = "Şu anda başvuruya açık bir eğitim bulunmuyor."
        },
        new()
        {
            Key = "applicationThanks",
            Group = Applications,
            Label = "Başvuru sonrası teşekkür metni",
            Hint = "Eğitim, etkinlik ve hackathon başvurusu gönderildikten sonra görünür.",
            Kind = SiteTextKind.LongText,
            MaxLength = 500,
            Placeholder = "Başvurunuz için teşekkür ederiz."
        },
        new()
        {
            Key = "kvkkText",
            Group = Applications,
            Label = "Hackathon aydınlatma (KVKK) metni",
            Hint = "Hackathon başvuru formunda gösterilir ve onaylatılır.",
            Kind = SiteTextKind.LongText,
            MaxLength = 10000
        },
        new()
        {
            Key = "certificateNote",
            Group = OtherPages,
            Label = "Sertifika doğrulama notu",
            Hint = "Geçerli sertifikanın altında, doğrulama kodunun önünde görünür. Ör. \"Bu sertifika ... tarafından verilmiştir.\"",
            Kind = SiteTextKind.LongText,
            MaxLength = 300
        },
        new()
        {
            Key = "counterIdleTitle",
            Group = OtherPages,
            Label = "Geri sayım ekranı (geri sayım yokken)",
            Hint = "Panelde geri sayım ayarlı değilken /counter ekranında görünür.",
            MaxLength = 100,
            Placeholder = "Hoş geldiniz!"
        }
    ];

    private static readonly Dictionary<string, SiteTextDefinition> ByKey = All.ToDictionary(d => d.Key);

    public static SiteTextDefinition? Find(string key) => ByKey.GetValueOrDefault(key);
}
