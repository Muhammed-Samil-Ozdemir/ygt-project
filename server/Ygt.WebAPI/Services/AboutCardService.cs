using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class AboutCardService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
    public async Task<List<AboutCardListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.AboutCards
            .OrderBy(c => c.SortOrder)
            .Select(c => new AboutCardListItemDto(c.Id, c.Title, c.Text, c.ImageFileName, c.IsVisible))
            .ToListAsync(cancellationToken);

    /// <summary>Yeni kart listenin sonuna eklenir. Görsel ayrıca yüklenir.</summary>
    public async Task<AboutCardListItemDto> CreateAsync(AboutCardCommandDto request, CancellationToken cancellationToken)
    {
        var card = new AboutCard { SortOrder = await Ordering.NextAsync(dbContext.AboutCards, cancellationToken) };
        Fill(card, request);

        dbContext.AboutCards.Add(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToListItem(card);
    }

    public async Task UpdateAsync(Guid id, AboutCardCommandDto request, CancellationToken cancellationToken)
    {
        var card = await FindAsync(id, cancellationToken);
        Fill(card, request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await FindAsync(id, cancellationToken);
        dbContext.AboutCards.Remove(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, card.ImageFileName);
    }

    /// <summary>Görseli yükler, öncekini siler. Kartın güncel hâlini döndürür.</summary>
    public async Task<AboutCardListItemDto> UploadImageAsync(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        var card = await FindAsync(id, cancellationToken);
        var oldFileName = card.ImageFileName;

        card.ImageFileName = await imageStorage.SaveNewAsync(ImageStorageProvider.Site, file, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);

        return ToListItem(card);
    }

    public async Task DeleteImageAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await FindAsync(id, cancellationToken);
        var oldFileName = card.ImageFileName;

        card.ImageFileName = null;
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);
    }

    public async Task ReorderAsync(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await Ordering.ApplyAsync(dbContext.AboutCards, request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Fill(AboutCard card, AboutCardCommandDto request)
    {
        card.Title = Input.RequiredText(request.Title, "Başlık", 2, 100);
        card.Text = Input.RequiredText(request.Text, "Metin", 10, 1000);
        card.IsVisible = request.IsVisible ?? true;
    }

    private static AboutCardListItemDto ToListItem(AboutCard card) =>
        new(card.Id, card.Title, card.Text, card.ImageFileName, card.IsVisible);

    private async Task<AboutCard> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.AboutCards.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Kart bulunamadı.");
}
