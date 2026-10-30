using Ygt.WebAPI.Enums;

namespace Ygt.WebAPI.Context;

/// <summary>
/// Seed:SampleContent açıkken ilk kurulumda temel içeriğe ek olarak eklenen örnek veri. Site ve panel ekranları
/// denerken dolu görünsün diye var; sunucuda kapalı kalır.
/// Ekip ve etkinlik fotoğrafları eski siteden alındı (SeedData/sample). Başvuru ve sertifikalardaki kişiler uydurmadır.
/// </summary>
public static class SampleContent
{
    /// <summary>Eski sitedeki yönetim kurulu. Fotoğraflar SeedData/sample/team altında.</summary>
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

    /// <summary>
    /// Eski sitenin galerisindeki etkinlikler. Fotoğraflar SeedData/sample/events/{Folder} altında; dosya adı sırasıyla
    /// eklenir, ilki kapak olur. Açıklamalar örnek olarak yazıldı.
    /// </summary>
    public static readonly (string Folder, string Title, string? Description, EventStyle Style)[] Events =
    [
        ("hsd", "HSD etkinliği", "Konuşmalar ve soru-cevap bölümüyle geçen, salonu dolduran bir etkinlik.", EventStyle.Featured),
        ("gdg", "GDG etkinliği", null, EventStyle.Wall),
        ("nibgat-1", "NİBGAT semineri", null, EventStyle.Wall),
        ("nibgat-2", "NİBGAT semineri #2", null, EventStyle.Wall),
        ("tubitak", "TÜBİTAK semineri", null, EventStyle.Wall),
        ("coffee-talk", "Coffee Talk", "Kahve eşliğinde sohbet ve tanışma buluşmaları.", EventStyle.Recurring),
        ("bowling", "Bowling", "Üyelerimizle birlikte bowling buluşması.", EventStyle.Recurring)
    ];

    public const string CourseTitle = "Web Geliştirmeye Giriş";
    public const string CourseContent = "Örnek eğitim. HTML, CSS ve JavaScript temelleri.";

    public const string FormTitle = "Kariyer Sohbetleri";
    public const string FormContent = "Örnek etkinlik formu.";

    public const string HackathonTitle = "YGT Hackathon";
    public const string HackathonContent = "Örnek hackathon formu (başvuruya kapalı).";

    /// <summary>Başvurularda ve sertifikalarda kullanılan uydurma kişiler. Telefonlar 10 hane, e-postalar küçük harf.</summary>
    public static readonly (string FirstName, string LastName, string Email, string Phone)[] People =
    [
        ("Ahmet", "Yılmaz", "ahmet.yilmaz@example.com", "5550000001"),
        ("Elif", "Demir", "elif.demir@example.com", "5550000002"),
        ("Mehmet", "Kaya", "mehmet.kaya@example.com", "5550000003"),
        ("Zeynep", "Çelik", "zeynep.celik@example.com", "5550000004"),
        ("Can", "Arslan", "can.arslan@example.com", "5550000005"),
        ("Merve", "Şahin", "merve.sahin@example.com", "5550000006")
    ];
}
