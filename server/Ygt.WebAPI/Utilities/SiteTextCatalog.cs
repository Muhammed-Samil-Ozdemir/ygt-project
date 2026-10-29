using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Utilities;

/// <summary>Panelden değiştirilebilen bir site metninin tanımı. Panel alanı bu bilgilerle çizer.</summary>
public sealed record SiteTextDefinition
{
    /// <summary>Veritabanındaki ve ön yüzdeki anahtar, ör. "heroTitle".</summary>
    public required string Key { get; init; }

    /// <summary>Panelde alanların gruplandığı başlık.</summary>
    public required string Group { get; init; }

    public required string Label { get; init; }
    public string? Hint { get; init; }
    public SiteTextKind Kind { get; init; } = SiteTextKind.Text;
    public required int MaxLength { get; init; }
    public bool IsRequired { get; init; }

    /// <summary>İlk kurulumda veritabanına yazılan değer (eski sitedeki içerik).</summary>
    public required string DefaultValue { get; init; }
}

/// <summary>
/// Sitedeki tek parça metinlerin listesi. Yeni metin eklemek için buraya tanım eklemek yeterli:
/// API açılışta eksik anahtarı varsayılan değeriyle veritabanına yazar, panel alanı kendiliğinden gösterir.
/// Ön yüzde kullanmak için libraries'deki SiteTexts arayüzüne de eklenmeli.
/// </summary>
public static class SiteTextCatalog
{
    private const string General = "Genel";
    private const string Hero = "Ana sayfa girişi";
    private const string Join = "Bize katıl";
    private const string Footer = "Alt bilgi";
    private const string Applications = "Başvurular";

    public static readonly IReadOnlyList<SiteTextDefinition> All =
    [
        new()
        {
            Key = "siteName",
            Group = General,
            Label = "Topluluk adı",
            Hint = "Üst menüde ve alt bilgide logonun yanında görünür. İlk kelime turuncu yazılır.",
            MaxLength = 100,
            IsRequired = true,
            DefaultValue = "Yazılım Geliştirme Topluluğu"
        },
        new()
        {
            Key = "seoTitle",
            Group = General,
            Label = "Sayfa başlığı",
            Hint = "Tarayıcı sekmesinde ve arama sonuçlarında görünür.",
            MaxLength = 100,
            IsRequired = true,
            DefaultValue = "Yazılım Geliştirme Topluluğu | KTÜN"
        },
        new()
        {
            Key = "seoDescription",
            Group = General,
            Label = "Sayfa açıklaması",
            Hint = "Arama sonuçlarında başlığın altında görünür.",
            Kind = SiteTextKind.LongText,
            MaxLength = 300,
            DefaultValue = "Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu: eğitimler, etkinlikler ve hackathon başvuruları."
        },
        new()
        {
            Key = "heroEyebrow",
            Group = Hero,
            Label = "Üst başlık",
            Hint = "Büyük başlığın üstündeki küçük yazı. Boş bırakılırsa gösterilmez.",
            MaxLength = 100,
            DefaultValue = "Konya Teknik Üniversitesi"
        },
        new()
        {
            Key = "heroTitle",
            Group = Hero,
            Label = "Başlık",
            Hint = "İlk kelime turuncu yazılır.",
            MaxLength = 150,
            IsRequired = true,
            DefaultValue = "Yazılım Geliştirme Topluluğu"
        },
        new()
        {
            Key = "heroLead",
            Group = Hero,
            Label = "Açıklama",
            Kind = SiteTextKind.LongText,
            MaxLength = 500,
            DefaultValue = "Web, mobil ve oyun geliştirmeye ilgi duyan öğrencilerin birlikte öğrendiği, proje ürettiği ve etkinlikler düzenlediği topluluk."
        },
        new()
        {
            Key = "heroSecondaryButton",
            Group = Hero,
            Label = "İkinci buton",
            Hint = "Hakkımızda bölümüne kaydırır. Boş bırakılırsa gösterilmez.",
            MaxLength = 40,
            DefaultValue = "Bizi tanıyın"
        },
        new()
        {
            Key = "joinButton",
            Group = Join,
            Label = "Buton yazısı",
            Hint = "Üst menüde, girişte ve \"Bize katıl\" kartında görünür.",
            MaxLength = 40,
            IsRequired = true,
            DefaultValue = "Bize Katılın"
        },
        new()
        {
            Key = "joinUrl",
            Group = Join,
            Label = "Katılım formu bağlantısı",
            Hint = "Boş bırakılırsa \"Bize Katılın\" butonları gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500,
            DefaultValue = "https://docs.google.com/forms/d/e/1FAIpQLSdo7ylOulj-Oa3HydpB8tppgShYSQXXJdHhEpy8vmcAp2XiKg/viewform"
        },
        new()
        {
            Key = "joinTitle",
            Group = Join,
            Label = "Kart başlığı",
            MaxLength = 100,
            IsRequired = true,
            DefaultValue = "Bize katıl!"
        },
        new()
        {
            Key = "joinText",
            Group = Join,
            Label = "Kart metni",
            Kind = SiteTextKind.LongText,
            MaxLength = 1000,
            DefaultValue = "Web sitesi ve mobil uygulama geliştirmeyi öğrenmek istiyorsan ya da bu alanda deneyimin varsa, yeteneklerin ve hedeflerin doğrultusunda değerlendirebileceğin birçok fırsat seni bekliyor."
        },
        new()
        {
            Key = "footerText",
            Group = Footer,
            Label = "Tanıtım metni",
            Kind = SiteTextKind.LongText,
            MaxLength = 300,
            DefaultValue = "Konya Teknik Üniversitesi bünyesinde 2022'de kurulan öğrenci topluluğu."
        },
        new()
        {
            Key = "universityName",
            Group = Footer,
            Label = "Üniversite bağlantısının yazısı",
            MaxLength = 100,
            DefaultValue = "Konya Teknik Üniversitesi"
        },
        new()
        {
            Key = "universityUrl",
            Group = Footer,
            Label = "Üniversite bağlantısı",
            Hint = "Boş bırakılırsa gösterilmez.",
            Kind = SiteTextKind.Url,
            MaxLength = 500,
            DefaultValue = "https://www.ktun.edu.tr/"
        },
        new()
        {
            Key = "applicationThanks",
            Group = Applications,
            Label = "Başvuru sonrası teşekkür metni",
            Hint = "Eğitim, etkinlik ve hackathon başvurusu gönderildikten sonra görünür.",
            Kind = SiteTextKind.LongText,
            MaxLength = 500,
            IsRequired = true,
            DefaultValue = "Teşekkür ederiz. Gelişmeleri sosyal medya hesaplarımızdan duyuracağız."
        },
        new()
        {
            Key = "kvkkText",
            Group = Applications,
            Label = "Hackathon aydınlatma (KVKK) metni",
            Hint = "Hackathon başvuru formunda onaylatılır.",
            Kind = SiteTextKind.LongText,
            MaxLength = 10000,
            IsRequired = true,
            DefaultValue = KvkkText
        }
    ];

    private static readonly Dictionary<string, SiteTextDefinition> ByKey = All.ToDictionary(d => d.Key);

    public static SiteTextDefinition? Find(string key) => ByKey.GetValueOrDefault(key);

    // Eski sitedeki hackathon aydınlatma metni.
    private const string KvkkText =
        """
        Bu Aydınlatma Metni, 6698 sayılı Kişisel Verilerin Korunması Kanunu ("KVKK") uyarınca, veri sorumlusu sıfatıyla Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu ("Topluluk") tarafından hazırlanmıştır.

        1. İşlenen kişisel veriler
        Başvuru formu aracılığıyla ad soyad, e-posta adresi ve telefon numarası bilgileriniz toplanmaktadır.

        2. Kişisel verilerin işlenme amacı
        Toplanan kişisel verileriniz yalnızca hackathon başvurularının alınması ve değerlendirilmesi, etkinlik hakkında bilgilendirme yapılması ve katılımcılarla iletişimin sağlanması amaçlarıyla işlenecektir.

        3. Kişisel verilerin işlenmesinin hukuki sebebi
        Kişisel verileriniz; bir sözleşmenin kurulması ve ifasıyla doğrudan ilgili olması (başvuru ve etkinlik katılım sürecinin yürütülmesi, KVKK m.5/2-c) ve bir hakkın tesisi, kullanılması veya korunması için veri işlemenin zorunlu olması (KVKK m.5/2-e) hukuki sebeplerine dayanılarak işlenecektir.

        4. Kişisel verilerin aktarılması
        Kişisel verileriniz, yalnızca etkinliğin yürütülmesi amacıyla sınırlı olmak üzere Topluluk yetkilileri ve etkinlik sürecinde görev alan organizasyon ekibi üyeleriyle paylaşılabilecektir. İzniniz olmadan üçüncü taraf kurum ve kuruluşlarla paylaşılmayacaktır.

        5. Kişisel verilerin saklama süresi
        Kişisel verileriniz etkinlik süresince ve etkinlik sonrasında en fazla 6 ay süreyle saklanacak, ardından güvenli bir şekilde imha edilecektir.

        6. İlgili kişinin hakları
        KVKK'nın 11. maddesi kapsamında kişisel verilerinizin işlenip işlenmediğini öğrenme, işlenme amacını ve amacına uygun kullanılıp kullanılmadığını öğrenme, yurt içi veya yurt dışında aktarıldığı üçüncü kişileri öğrenme, eksik veya yanlış işlenmişse düzeltilmesini isteme, kanunda öngörülen şartlarda silinmesini veya yok edilmesini isteme, münhasıran otomatik sistemlerle analiz edilmesi sonucu aleyhinize bir sonuç çıkmasına itiraz etme ve zarara uğramanız halinde zararın giderilmesini talep etme haklarına sahipsiniz.

        Başvurularınızı yazilimgelistirme@ktun.edu.tr adresine iletebilirsiniz.

        Veri sorumlusu: Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu
        E-posta: yazilimgelistirme@ktun.edu.tr
        """;
}
