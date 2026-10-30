using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class SiteService(AppDbContext dbContext)
{
    public async Task<SiteSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await dbContext.SiteSettings.AsNoTracking().FirstAsync(cancellationToken);

        var countdown = settings is { CountdownTitle: not null, CountdownEndsAt: not null }
            ? new CountdownDto(settings.CountdownTitle, settings.CountdownEndsAt.Value)
            : null;

        var applicationOpen = settings.HomeButton == HomeButton.Course
            ? await dbContext.Courses.AnyAsync(c => c.IsActive, cancellationToken)
            : await dbContext.Forms.AnyAsync(f => f.IsActive, cancellationToken);

        return new SiteSettingsDto(settings.HomeButton, countdown, applicationOpen);
    }

    public async Task UpdateSettingsAsync(SiteSettingsCommandDto request, CancellationToken cancellationToken)
    {
        if (request.HomeButton is null)
            throw Errors.BadRequest("Ana sayfa butonu seçilmelidir.");

        var countdownTitle = Input.OptionalText(request.CountdownTitle, "Geri sayım başlığı", 255);
        if ((countdownTitle is null) != (request.CountdownEndsAt is null))
            throw Errors.BadRequest("Geri sayım için başlık ve bitiş tarihi birlikte girilmelidir.");

        var settings = await dbContext.SiteSettings.FirstAsync(cancellationToken);
        settings.HomeButton = request.HomeButton.Value;
        settings.CountdownTitle = countdownTitle;
        settings.CountdownEndsAt = request.CountdownEndsAt;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Sayfanın sayacını bir artırır. Sadece tanımlı sayfalar sayılır (şimdilik "home"),
    /// böylece rastgele sayfa adlarıyla tabloya kayıt eklenemez.
    /// </summary>
    public Task RecordVisitAsync(string page, CancellationToken cancellationToken) =>
        dbContext.VisitorCounters
            .Where(c => c.Page == page)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.Count, c => c.Count + 1)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow), cancellationToken);

    public async Task<List<VisitCounterDto>> GetVisitsAsync(CancellationToken cancellationToken) =>
        await dbContext.VisitorCounters
            .OrderBy(c => c.Page)
            .Select(c => new VisitCounterDto(c.Page, c.Count))
            .ToListAsync(cancellationToken);
}
