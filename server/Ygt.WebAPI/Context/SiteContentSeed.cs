namespace Ygt.WebAPI.Context;

/// <summary>
/// İlk kurulumda veritabanına yazılan liste içeriği (eski sitedeki içerik). Tek parça metinlerin varsayılanları
/// SiteTextCatalog'da. Ekip fotoğrafları SeedData/team klasöründen storage'a kopyalanır.
/// </summary>
public static class SiteContentSeed
{
    public static readonly (string Title, string Text)[] About =
    [
        ("Biz kimiz?", "Konya Teknik Üniversitesi Yazılım Geliştirme Topluluğu, bilgisayar ve yazılım mühendisliği öğrencileri tarafından 2022 yılında kurulmuştur. Web, mobil ve oyun alanlarına ilgi duyan öğrencilerin bir araya gelerek çalışma grupları oluşturduğu, bilgi alışverişinde bulunduğu ve kendini bu alanlarda geliştirdiği bir platformdur."),
        ("Neler yaparız?", "Web, mobil ve oyun alanlarında ayrı çalışma grupları oluşturuyor, bu grupları ödevler ve derslerle destekliyoruz. Alanında uzman kişilerin seminerleriyle üyelerimizin bilgi ve deneyimini artırıyor, kaynaşmak için sosyal etkinlikler de düzenliyoruz."),
        ("Neden biz?", "Topluluğumuz yazılım alanında gelişmenin yanında güzel arkadaşlıklar kurabileceğin bir ortam sunar. Birlikte yaptığımız projelerle CV’ne ekleyebileceğin deneyimler kazanır, üniversite yıllarını daha aktif ve verimli geçirirsin.")
    ];

    public static readonly (string Name, string Role, string Photo)[] Team =
    [
        ("Esra Tosun", "Yönetim Kurulu Başkanı", "esra.jpg"),
        ("Ayşe Melek Acar", "Yönetim Kurulu Başkan Yardımcısı", "ayse.jpg"),
        ("Ömer Şahin", "Yönetim Kurulu Başkan Yardımcısı", "omer.jpg"),
        ("Baiysh Sabirzhanov", "Yönetim Kurulu Üyesi", "baiysh.jpg"),
        ("Büşra Kaya", "Yönetim Kurulu Üyesi", "busra.jpg"),
        ("Şaziye Dağ", "Yönetim Kurulu Üyesi", "saziye.jpg"),
        ("Nisa Peri Aksoy", "Yönetim Kurulu Üyesi", "nisaPeri.jpg"),
        ("İbrahim Halil Altundağ", "Yönetim Kurulu Üyesi", "ibrahim.jpg"),
        ("Şeyda Açıkgöz", "Yönetim Kurulu Üyesi", "seyda.jpg"),
        ("Fatma Zehra Doğan", "Yönetim Kurulu Üyesi", "fatmaZehra.jpg"),
        ("Feyza Doğmuş", "Yönetim Kurulu Üyesi", "feyza.jpg"),
        ("Ayşegül Doğan", "Yönetim Kurulu Üyesi", "aysegul.jpg")
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
}
