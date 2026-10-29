using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

/// <summary>Sitenin içeriği: siteye giden tek parça yanıt, sitenin görselleri ve paneldeki metinler.</summary>
public sealed class SiteContentService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
    public async Task<SiteContentDto> GetAsync(CancellationToken cancellationToken)
    {
        var stored = await ReadTextsAsync(cancellationToken);
        var texts = SiteTextCatalog.All.ToDictionary(
            d => d.Key,
            d => stored.GetValueOrDefault(d.Key) is { Length: > 0 } value ? value : d.Placeholder ?? "");

        var about = await dbContext.AboutCards
            .Where(c => c.IsVisible)
            .OrderBy(c => c.SortOrder)
            .Select(c => new AboutCardDto(c.Title, c.Text, c.ImageFileName))
            .ToListAsync(cancellationToken);

        var events = await dbContext.Events
            .Where(e => e.IsVisible && e.Photos.Any())
            .OrderBy(e => e.SortOrder)
            .Select(e => new EventDto(
                e.Title,
                e.Description,
                e.Style,
                e.Photos.OrderBy(p => p.SortOrder).Select(p => p.FileName).ToList()))
            .ToListAsync(cancellationToken);

        var team = await dbContext.TeamMembers
            .Where(m => m.IsVisible)
            .OrderBy(m => m.SortOrder)
            .Select(m => new TeamMemberDto(m.Name, m.Role, m.PhotoFileName))
            .ToListAsync(cancellationToken);

        var technologies = await dbContext.Technologies
            .OrderBy(t => t.SortOrder)
            .Select(t => t.Name)
            .ToListAsync(cancellationToken);

        var links = await dbContext.SiteLinks
            .Where(l => l.IsVisible)
            .OrderBy(l => l.SortOrder)
            .Select(l => new SiteLinkDto(l.Label, l.Url))
            .ToListAsync(cancellationToken);

        return new SiteContentDto(texts, about, events, team, technologies, links);
    }

    /// <summary>Sitenin görseli. Klasörde sadece panelden yüklenen, sitede herkese görünen görseller durur.</summary>
    public Task<FileDto> GetImageAsync(string fileName, CancellationToken cancellationToken) =>
        imageStorage.ReadAsync(ImageStorageProvider.Site, fileName, "Görsel bulunamadı.", cancellationToken);

    public async Task<List<SiteTextDto>> GetTextsAsync(CancellationToken cancellationToken)
    {
        var values = await ReadTextsAsync(cancellationToken);
        return SiteTextCatalog.All.Select(d => ToDto(d, values.GetValueOrDefault(d.Key, ""))).ToList();
    }

    /// <summary>Metinleri kaydeder. Görseller bu yoldan değil, UploadImageAsync ile değişir.</summary>
    public async Task UpdateTextsAsync(SiteTextsCommandDto request, CancellationToken cancellationToken)
    {
        if (request.Values is null || request.Values.Count == 0)
            throw Errors.BadRequest("Değiştirilecek metin gönderilmedi.");

        var values = request.Values.ToDictionary(pair => pair.Key, pair => Clean(FindText(pair.Key), pair.Value));

        var keys = values.Keys.ToList();
        var rows = await dbContext.SiteTexts
            .Where(t => keys.Contains(t.Key))
            .ToDictionaryAsync(t => t.Key, cancellationToken);

        foreach (var (key, value) in values)
        {
            if (rows.TryGetValue(key, out var row))
                row.Value = value;
            else
                dbContext.SiteTexts.Add(new SiteText { Key = key, Value = value });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Görseli yükler, öncekini siler. Alanın güncel hâlini döndürür.</summary>
    public async Task<SiteTextDto> UploadImageAsync(string key, IFormFile? file, CancellationToken cancellationToken)
    {
        var definition = FindImage(key);

        var row = await dbContext.SiteTexts.FirstOrDefaultAsync(t => t.Key == key, cancellationToken);
        if (row is null)
        {
            row = new SiteText { Key = key };
            dbContext.SiteTexts.Add(row);
        }

        var oldFileName = row.Value;
        row.Value = await imageStorage.SaveNewAsync(ImageStorageProvider.Site, file, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);

        return ToDto(definition, row.Value);
    }

    public async Task DeleteImageAsync(string key, CancellationToken cancellationToken)
    {
        FindImage(key);
        var row = await dbContext.SiteTexts.FirstOrDefaultAsync(t => t.Key == key, cancellationToken);
        if (row is null || row.Value.Length == 0)
            return;

        var oldFileName = row.Value;
        row.Value = "";
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);
    }

    /// <summary>Metni tanımındaki kurallara göre doğrular. Boş bırakılan metin "" olarak saklanır.</summary>
    private static string Clean(SiteTextDefinition definition, string? value)
    {
        var label = $"{definition.Label} ({definition.Group})";

        var text = definition.Kind == SiteTextKind.Url
            ? Input.OptionalUrl(value, label)
            : Input.OptionalText(value, label, definition.MaxLength);

        return text ?? "";
    }

    private static SiteTextDefinition FindText(string key)
    {
        var definition = SiteTextCatalog.Find(key) ?? throw Errors.BadRequest($"Bilinmeyen alan: {key}");
        if (definition.Kind == SiteTextKind.Image)
            throw Errors.BadRequest($"{definition.Label} bir görseldir; metin olarak değiştirilemez.");

        return definition;
    }

    private static SiteTextDefinition FindImage(string key)
    {
        var definition = SiteTextCatalog.Find(key) ?? throw Errors.NotFound($"Bilinmeyen alan: {key}");
        if (definition.Kind != SiteTextKind.Image)
            throw Errors.BadRequest($"{definition.Label} bir görsel alanı değildir.");

        return definition;
    }

    private static SiteTextDto ToDto(SiteTextDefinition d, string value) =>
        new(d.Key, d.Group, d.Label, d.Hint, d.Kind, d.MaxLength, d.Placeholder, value);

    private async Task<Dictionary<string, string>> ReadTextsAsync(CancellationToken cancellationToken) =>
        await dbContext.SiteTexts.AsNoTracking().ToDictionaryAsync(t => t.Key, t => t.Value, cancellationToken);
}
