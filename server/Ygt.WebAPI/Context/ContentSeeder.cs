using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;

namespace Ygt.WebAPI.Context;

/// <summary>
/// Başlangıç içeriğini (DefaultContent) ve örnek veriyi (SampleContent) DbContext'e ekler; kaydetmez.
/// DbInitializer bunu sadece ilk kurulumda, site ayarlarının satırıyla birlikte çağırır ve hepsini tek seferde kaydeder.
/// Görseller SeedData klasöründen storage'a kopyalanır; panelden yüklenenler gibi değiştirilip silinebilir.
/// </summary>
public sealed class ContentSeeder(AppDbContext dbContext, ImageStorageProvider imageStorage, ILogger logger)
{
    private static readonly string SeedDataPath = Path.Combine(AppContext.BaseDirectory, "SeedData");

    public void AddDefaultContent()
    {
        // Kaynak dosya Windows'ta CRLF ile açılmış olabilir; panelden kaydedilen metinler gibi \n kullanılır.
        foreach (var (key, value) in DefaultContent.Texts)
            dbContext.SiteTexts.Add(new SiteText { Key = key, Value = value.ReplaceLineEndings("\n") });

        foreach (var (key, file) in DefaultContent.Images)
        {
            if (CopyImage(Path.Combine("default", file)) is { } fileName)
                dbContext.SiteTexts.Add(new SiteText { Key = key, Value = fileName });
        }

        dbContext.AboutCards.AddRange(DefaultContent.About.Select((card, index) =>
            new AboutCard { Title = card.Title, Text = card.Text, SortOrder = index }));

        dbContext.Technologies.AddRange(DefaultContent.Technologies.Select((name, index) =>
            new Technology { Name = name, SortOrder = index }));

        dbContext.SiteLinks.AddRange(DefaultContent.Links.Select((link, index) =>
            new SiteLink { Label = link.Label, Url = link.Url, SortOrder = index }));
    }

    public void AddSampleContent()
    {
        dbContext.TeamMembers.AddRange(SampleContent.Team.Select((member, index) => new TeamMember
        {
            Name = member.Name,
            Role = member.Role,
            PhotoFileName = CopyImage(Path.Combine("sample", "team", member.Photo)),
            SortOrder = index
        }));

        dbContext.Events.AddRange(SampleContent.Events.Select((item, index) => new Event
        {
            Title = item.Title,
            Description = item.Description,
            Style = item.Style,
            SortOrder = index,
            Photos = EventPhotos(item.Folder)
        }));

        var people = SampleContent.People;

        dbContext.Courses.Add(new Course
        {
            Title = SampleContent.CourseTitle,
            Content = SampleContent.CourseContent,
            Applications = people.Take(4).Select(p => new CourseApplication
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.Phone
            }).ToList()
        });

        // Aynı anda tek form açık olabilir: etkinlik formu açık, hackathon formu kapalı eklenir.
        dbContext.Forms.Add(new Form
        {
            Title = SampleContent.FormTitle,
            Content = SampleContent.FormContent,
            Applications = people.Skip(2).Select(p => new FormApplication
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.Phone
            }).ToList()
        });

        dbContext.Forms.Add(new Form
        {
            Title = SampleContent.HackathonTitle,
            Content = SampleContent.HackathonContent,
            Type = FormType.Hackathon,
            IsActive = false,
            HackathonApplications =
            [
                Team("Kod Kaşifleri", people[0], people[1], people[2]),
                Team("Bug Avcıları", people[3], people[4], null)
            ]
        });

        dbContext.Certificates.AddRange(people.Take(3).Select(p => new Certificate
        {
            Code = Guid.NewGuid().ToString(),
            CourseName = SampleContent.CourseTitle,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            PhoneNumber = p.Phone
        }));
    }

    private List<EventPhoto> EventPhotos(string folder)
    {
        var directory = Path.Combine(SeedDataPath, "sample", "events", folder);
        if (!Directory.Exists(directory))
        {
            logger.LogWarning("Örnek etkinlik fotoğrafları bulunamadı: {Path}", directory);
            return [];
        }

        return Directory.GetFiles(directory)
            .Order(StringComparer.Ordinal)
            .Select((path, index) => new EventPhoto
            {
                FileName = imageStorage.CopyNew(ImageStorageProvider.Site, path),
                SortOrder = index
            })
            .ToList();
    }

    private static HackathonApplication Team(
        string name,
        (string FirstName, string LastName, string Email, string Phone) member1,
        (string FirstName, string LastName, string Email, string Phone) member2,
        (string FirstName, string LastName, string Email, string Phone)? member3) => new()
    {
        TeamName = name,
        Member1Name = $"{member1.FirstName} {member1.LastName}",
        Member1Email = member1.Email,
        Member1Phone = member1.Phone,
        Member2Name = $"{member2.FirstName} {member2.LastName}",
        Member2Email = member2.Email,
        Member2Phone = member2.Phone,
        Member3Name = member3 is { } m3 ? $"{m3.FirstName} {m3.LastName}" : null,
        Member3Email = member3?.Email,
        Member3Phone = member3?.Phone
    };

    /// <summary>SeedData altındaki görseli sitenin görsel klasörüne kopyalar. Dosya yoksa uyarı yazar, boş döner.</summary>
    private string? CopyImage(string relativePath)
    {
        var source = Path.Combine(SeedDataPath, relativePath);
        if (File.Exists(source))
            return imageStorage.CopyNew(ImageStorageProvider.Site, source);

        logger.LogWarning("Başlangıç görseli bulunamadı: {Path}", source);
        return null;
    }
}
