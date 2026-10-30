namespace Ygt.WebAPI.Context;

/// <summary>
/// İlk kurulumda her ortamda (production dahil) eklenen temel içerik. Metinler eski siteden alındı. Amaç sitenin
/// boş açılmaması; hepsi panelden değiştirilip silinebilir.
/// Burada olmayan metinler boş başlar; sitede ya katalogdaki genel ifade (ör. "Hakkımızda") yazılır ya da
/// o alan gösterilmez. Ekip ve etkinlikler kişilere ve döneme özel olduğu için burada yok, SampleContent'te.
/// </summary>
public static class DefaultContent
{
    /// <summary>SiteTextCatalog anahtarı -> değer.</summary>
    public static readonly IReadOnlyDictionary<string, string> Texts = new Dictionary<string, string>
    {
        ["siteName"] = "Yazılım Geliştirme Topluluğu",
        ["seoTitle"] = "Yazılım Geliştirme Topluluğu | KTÜN",
        ["seoDescription"] = "Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu: eğitimler, etkinlikler ve hackathon başvuruları.",
        ["heroEyebrow"] = "Konya Teknik Üniversitesi",
        ["heroLead"] = "Web, mobil ve oyun geliştirmeye ilgi duyan öğrencilerin birlikte öğrendiği, proje ürettiği ve etkinlikler düzenlediği topluluk.",
        ["heroSecondaryButton"] = "Bizi tanıyın",
        ["joinUrl"] = "https://docs.google.com/forms/d/e/1FAIpQLSdo7ylOulj-Oa3HydpB8tppgShYSQXXJdHhEpy8vmcAp2XiKg/viewform",
        ["joinText"] = "Web sitesi ve mobil uygulama geliştirmeyi öğrenmek istiyorsan veya bu alanda deneyime sahipsen, yeteneklerin ve hedeflerin doğrultusunda değerlendirebileceğin birçok fırsat, Yazılım Geliştirme Topluluğu'nda seni bekliyor.",
        ["membershipUrl"] = "https://topluluk.ktun.edu.tr",
        ["gamesUrl"] = "https://ygt-mini-oyunlar.gt.tc",
        ["footerText"] = "Konya Teknik Üniversitesi bünyesinde 2022'de kurulan öğrenci topluluğu.",
        ["universityName"] = "Konya Teknik Üniversitesi",
        ["universityUrl"] = "https://www.ktun.edu.tr/",
        ["kvkkText"] = KvkkText,
        ["certificateNote"] = "Sertifika, Yazılım Geliştirme Topluluğu tarafından onaylıdır."
    };

    /// <summary>Görsel alanlar: SiteTextCatalog anahtarı -> SeedData/default altındaki dosya (eski sitenin logosu).</summary>
    public static readonly IReadOnlyDictionary<string, string> Images = new Dictionary<string, string>
    {
        ["logo"] = "logo.png",
        ["favicon"] = "favicon.png"
    };

    public static readonly (string Title, string Text)[] About =
    [
        ("Biz kimiz?", "Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu, bilgisayar ve yazılım mühendisliği öğrencileri tarafından 2022 yılında kurulmuştur. Bu topluluk, web, mobil ve oyun alanlarına ilgi duyan öğrencilerin bir araya gelerek çalışma grupları oluşturdukları, bilgi alışverişinde bulundukları ve kendilerini bu alanlarda geliştirdikleri bir platformdur. Aynı zamanda, konuda uzman kişiler tarafından düzenlenen seminerlerle üyelerine bilgi birikimini artırma fırsatı sunmaktadır."),
        ("Neler yaparız?", "Web, mobil ve oyun alanlarında ayrı ayrı çalışma grupları oluşturulmaktadır. Bu gruplar, üyelerin bilgi birikimini artırmak amacıyla çeşitli ödevlendirmeler ve derslerle desteklenmektedir. Aynı zamanda, konuda uzman kişiler tarafından düzenlenen seminerlerle üyeler, bilgi ve tecrübeleri ile buluşma ve paylaşma fırsatı elde etmektedir. Topluluk, sadece bilgi odaklı değil, aynı zamanda sosyal açıdan da bir araya gelmeyi ve kaynaşmayı amaçlayan çeşitli etkinliklere de ev sahipliği yapmaktadır."),
        ("Neden biz?", "Yazılım Geliştirme Topluluğu, sadece yazılım alanında kendinizi geliştirmenize değil, aynı zamanda sosyal açıdan güzel arkadaşlıklar kurabileceğiniz bir ortam sunmaktadır. Birlikte gerçekleştireceğimiz projelerle CV'nize ekleyebileceğiniz birçok tecrübe kazanma fırsatı bulabilirsiniz. Bu projeler ve çalışmalar, üniversite yıllarınızı daha aktif ve verimli geçirmenize olanak tanır.")
    ];

    public static readonly string[] Technologies =
    [
        "HTML", "CSS", "JavaScript", "React", "React Native", "Node.js", "NoSQL", "Java", "C#", "Unity", ".NET"
    ];

    public static readonly (string Label, string Url)[] Links =
    [
        ("Instagram", "https://www.instagram.com/ktunygt/"),
        ("X (Twitter)", "https://twitter.com/ktunygt"),
        ("LinkedIn", "https://www.linkedin.com/company/ktunygt/"),
        ("Discord", "https://discord.gg/NgKXpsyatr")
    ];

    // Eski sitedeki hackathon aydınlatma metni.
    private const string KvkkText =
        """
        Hackathon Başvuru Formu Kişisel Verilerin Korunması Aydınlatma Metni

        Bu Aydınlatma Metni, 6698 sayılı Kişisel Verilerin Korunması Kanunu ("KVKK") uyarınca, veri sorumlusu sıfatıyla Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu ("Topluluk") tarafından hazırlanmıştır.

        1. İşlenen Kişisel Veriler
        Başvuru formu aracılığıyla aşağıdaki kişisel verileriniz toplanmaktadır:
        - Ad – Soyad
        - E-posta Adresi
        - Telefon Numarası

        2. Kişisel Verilerin İşlenme Amacı
        Toplanan kişisel verileriniz, yalnızca aşağıdaki amaçlarla işlenecektir:
        - Hackathon başvurularının alınması ve değerlendirilmesi,
        - Etkinlik hakkında bilgilendirme yapılması,
        - Katılımcılarla iletişimin sağlanması.

        3. Kişisel Verilerin İşlenmesinin Hukuki Sebebi
        Kişisel verileriniz;
        - Bir sözleşmenin kurulması ve ifasıyla doğrudan ilgili olması (Başvuru ve etkinlik katılım sürecinin yürütülmesi) (KVKK m.5/2-c),
        - Bir hakkın tesisi, kullanılması veya korunması için veri işlemenin zorunlu olması (KVKK m.5/2-e),
        hukuki sebeplerine dayanılarak işlenecektir.

        4. Kişisel Verilerin Aktarılması
        Kişisel verileriniz, yalnızca etkinliğin yürütülmesi amacıyla sınırlı olmak üzere Topluluk yetkilileri ve etkinlik sürecinde görev alan organizasyon ekibi üyeleriyle paylaşılabilecektir. Herhangi bir şekilde üçüncü taraf kurum ve kuruluşlarla izniniz olmadan paylaşılmayacaktır.

        5. Kişisel Verilerin Saklama Süresi
        Kişisel verileriniz, etkinlik süresince ve etkinlik sonrasında en fazla 6 ay süreyle saklanacak; ardından güvenli bir şekilde imha edilecektir.

        6. İlgili Kişinin Hakları
        KVKK'nın 11. maddesi kapsamında kişisel verilerinizle ilgili;
        - İşlenip işlenmediğini öğrenme,
        - İşlenme amacını ve amacına uygun kullanılıp kullanılmadığını öğrenme,
        - Yurt içi veya yurt dışı üçüncü kişilere aktarılmışsa bunları öğrenme,
        - Eksik veya yanlış işlenmişse düzeltilmesini isteme,
        - Kanunda öngörülen şartlarda silinmesini veya yok edilmesini isteme,
        - İşlenen verilerin münhasıran otomatik sistemler vasıtasıyla analiz edilmesi suretiyle aleyhe bir sonucun ortaya çıkmasına itiraz etme,
        - Zarara uğramanız halinde zararın giderilmesini talep etme,
        haklarına sahipsiniz.

        Başvurularınızı yazilimgelistirme@ktun.edu.tr e-posta adresine iletebilirsiniz.

        Veri Sorumlusu: Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu
        E-posta: yazilimgelistirme@ktun.edu.tr
        """;
}
