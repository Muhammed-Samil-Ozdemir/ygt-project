using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Options;

namespace Ygt.WebAPI.Context;

/// <summary>
/// Uygulama açılırken veritabanını hazırlar: bekleyen migration'ları uygular ve sistemin ihtiyaç duyduğu kayıtları
/// (admin, site ayarları, ziyaret sayacı) ekler. Site içeriği eklenmez; panelden girilir. Her açılışta çalışır.
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
    }

    /// <summary>
    /// İlk kurulumda, hiç yönetici yokken ayarlardaki bilgilerle ilk yöneticiyi oluşturur. Sonraki açılışlarda
    /// yöneticilere dokunmaz; şifreler ve yeni yöneticiler panelden yönetilir.
    /// Şifre unutulursa Seed:Admin:ResetPassword=true ile bir kez başlatılarak ayardaki şifreye dönülür.
    /// </summary>
    private static async Task SeedAdminAsync(
        AppDbContext dbContext,
        AdminSeedOptions options,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var hasUsers = await dbContext.Users.AnyAsync(cancellationToken);
        if (hasUsers && !options.ResetPassword)
            return;

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            logger.LogWarning("Seed:Admin:Password ayarlanmamış, yönetici oluşturulmadı ya da şifresi sıfırlanmadı.");
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
            logger.LogInformation("Yönetici oluşturuldu: {Email}", options.Email);
            return;
        }

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(options.Password);
        admin.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogWarning(
            "Yöneticinin şifresi ayardaki şifreye döndürüldü: {Email}. Seed:Admin:ResetPassword ayarını şimdi kapatın.",
            options.Email);
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
}
