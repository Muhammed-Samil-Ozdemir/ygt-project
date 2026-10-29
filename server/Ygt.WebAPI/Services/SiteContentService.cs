using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

/// <summary>Sitenin içeriği: siteye giden tek parça yanıt, ekip fotoğrafları ve paneldeki metinler.</summary>
public sealed class SiteContentService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
    public async Task<SiteContentDto> GetAsync(CancellationToken cancellationToken)
    {
        var texts = await ReadTextsAsync(cancellationToken);

        var about = await dbContext.AboutCards
            .Where(c => c.IsVisible)
            .OrderBy(c => c.SortOrder)
            .Select(c => new AboutCardDto(c.Title, c.Text))
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

        return new SiteContentDto(texts, about, team, technologies, links);
    }

    /// <summary>Sadece bir ekip üyesine ait fotoğraf verilir; storage'daki başka dosyalara bu yoldan ulaşılamaz.</summary>
    public async Task<FileDto> GetPhotoAsync(string fileName, CancellationToken cancellationToken)
    {
        const string notFound = "Fotoğraf bulunamadı.";

        if (!await dbContext.TeamMembers.AnyAsync(m => m.PhotoFileName == fileName, cancellationToken))
            throw Errors.NotFound(notFound);

        return await imageStorage.ReadAsync(ImageStorageProvider.Team, fileName, notFound, cancellationToken);
    }

    public async Task<List<SiteTextDto>> GetTextsAsync(CancellationToken cancellationToken)
    {
        var values = await ReadTextsAsync(cancellationToken);

        return SiteTextCatalog.All
            .Select(d => new SiteTextDto(d.Key, d.Group, d.Label, d.Hint, d.Kind, d.MaxLength, d.IsRequired, values[d.Key]))
            .ToList();
    }

    public async Task UpdateTextsAsync(SiteTextsCommandDto request, CancellationToken cancellationToken)
    {
        if (request.Values is null || request.Values.Count == 0)
            throw Errors.BadRequest("Değiştirilecek metin gönderilmedi.");

        var values = request.Values.ToDictionary(
            pair => pair.Key,
            pair => Clean(SiteTextCatalog.Find(pair.Key) ?? throw Errors.BadRequest($"Bilinmeyen metin: {pair.Key}"), pair.Value));

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

    /// <summary>Metni tanımındaki kurallara göre doğrular. Boş bırakılan isteğe bağlı metin "" olarak saklanır.</summary>
    private static string Clean(SiteTextDefinition definition, string? value)
    {
        var label = $"{definition.Label} ({definition.Group})";

        var text = definition.Kind == SiteTextKind.Url
            ? Input.OptionalUrl(value, label)
            : Input.OptionalText(value, label, definition.MaxLength);

        if (text is null && definition.IsRequired)
            throw Errors.BadRequest($"{label} zorunludur.");

        return text ?? "";
    }

    /// <summary>Katalogdaki her anahtarın kayıtlı değeri; kayıt yoksa varsayılan değeri.</summary>
    private async Task<Dictionary<string, string>> ReadTextsAsync(CancellationToken cancellationToken)
    {
        var stored = await dbContext.SiteTexts.AsNoTracking()
            .ToDictionaryAsync(t => t.Key, t => t.Value, cancellationToken);

        return SiteTextCatalog.All.ToDictionary(d => d.Key, d => stored.GetValueOrDefault(d.Key, d.DefaultValue));
    }
}
