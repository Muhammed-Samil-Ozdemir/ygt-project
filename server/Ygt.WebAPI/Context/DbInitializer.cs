using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Options;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Context;

/// <summary>
/// Uygulama açılırken veritabanını hazırlar: bekleyen migration'ları uygular ve başlangıç verilerini ekler.
/// Her açılışta çalışır; var olan veriye dokunmaz.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbInitializer));
        var cancellationToken = app.Lifetime.ApplicationStopping;

        if (app.Configuration.GetValue("Database:MigrateOnStartup", true))
        {
            logger.LogInformation("Bekleyen migration'lar uygulanıyor.");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        var adminOptions = services.GetRequiredService<IOptions<AdminSeedOptions>>().Value;
        await SeedAdminAsync(dbContext, adminOptions, logger, cancellationToken);
        await SeedSiteAsync(dbContext, logger, cancellationToken);

        var imageStorage = services.GetRequiredService<ImageStorageProvider>();
        await SeedContentAsync(dbContext, imageStorage, logger, cancellationToken);
    }

    /// <summary>
    /// Admin kullanıcısını oluşturur. Kullanıcı zaten varsa adını ve şifresini ayarlardakiyle eşitler;
    /// şifreyi değiştirmek için ayarı değiştirip uygulamayı yeniden başlatmak yeterli.
    /// </summary>
    private static async Task SeedAdminAsync(
        AppDbContext dbContext,
        AdminSeedOptions options,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.Password))
        {
            logger.LogWarning("Seed:Admin:Password ayarlanmamış, admin kullanıcısı oluşturulmadı.");
            return;
        }

        var admin = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == options.Email, cancellationToken);

        if (admin is null)
        {
            dbContext.Users.Add(new User
            {
                Name = options.Name,
                Email = options.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(options.Password)
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Admin kullanıcısı oluşturuldu: {Email}", options.Email);
            return;
        }

        if (!admin.VerifyPassword(options.Password))
        {
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(options.Password);
        }

        admin.Name = options.Name;

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Admin kullanıcısı güncellendi: {Email}", options.Email);
        }
    }

    /// <summary>Site ayarlarının tek satırını ve ana sayfa ziyaret sayacını oluşturur.</summary>
    private static async Task SeedSiteAsync(AppDbContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        if (!await dbContext.SiteSettings.AnyAsync(cancellationToken))
        {
            dbContext.SiteSettings.Add(new SiteSetting());
            logger.LogInformation("Site ayarları oluşturuldu.");
        }

        if (!await dbContext.VisitorCounters.AnyAsync(c => c.Page == VisitorCounter.HomePage, cancellationToken))
        {
            dbContext.VisitorCounters.Add(new VisitorCounter { Page = VisitorCounter.HomePage });
            logger.LogInformation("Ana sayfa ziyaret sayacı oluşturuldu.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Site içeriğini hazırlar. Eksik metin anahtarları varsayılan değerleriyle eklenir; böylece SiteTextCatalog'a
    /// sonradan eklenen metinler de veritabanına yazılır. Listeler (hakkımızda, ekip, teknolojiler, bağlantılar)
    /// sadece ilk kurulumda doldurulur, panelden silinen kayıtlar geri gelmez.
    /// </summary>
    private static async Task SeedContentAsync(
        AppDbContext dbContext,
        ImageStorageProvider imageStorage,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var existingKeys = await dbContext.SiteTexts.Select(t => t.Key).ToListAsync(cancellationToken);
        var isFirstRun = existingKeys.Count == 0;

        foreach (var definition in SiteTextCatalog.All.Where(d => !existingKeys.Contains(d.Key)))
        {
            dbContext.SiteTexts.Add(new SiteText { Key = definition.Key, Value = definition.DefaultValue });
        }

        if (isFirstRun)
        {
            dbContext.AboutCards.AddRange(SiteContentSeed.About.Select((card, index) =>
                new AboutCard { Title = card.Title, Text = card.Text, SortOrder = index }));

            dbContext.TeamMembers.AddRange(SiteContentSeed.Team.Select((member, index) =>
                new TeamMember
                {
                    Name = member.Name,
                    Role = member.Role,
                    PhotoFileName = CopySeedPhoto(imageStorage, member.Photo, logger),
                    SortOrder = index
                }));

            dbContext.Technologies.AddRange(SiteContentSeed.Technologies.Select((name, index) =>
                new Technology { Name = name, SortOrder = index }));

            dbContext.SiteLinks.AddRange(SiteContentSeed.Links.Select((link, index) =>
                new SiteLink { Label = link.Label, Url = link.Url, SortOrder = index }));

            logger.LogInformation("Site içeriği başlangıç verisiyle dolduruldu.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Başlangıç fotoğrafını storage'a yeni bir adla kopyalar. Dosya yoksa kişi fotoğrafsız eklenir.</summary>
    private static string? CopySeedPhoto(ImageStorageProvider imageStorage, string photo, ILogger logger)
    {
        var source = Path.Combine(AppContext.BaseDirectory, "SeedData", "team", photo);
        if (!File.Exists(source))
        {
            logger.LogWarning("Başlangıç fotoğrafı bulunamadı: {Path}", source);
            return null;
        }

        return imageStorage.Copy(ImageStorageProvider.Team, Guid.NewGuid().ToString(), source);
    }
}
