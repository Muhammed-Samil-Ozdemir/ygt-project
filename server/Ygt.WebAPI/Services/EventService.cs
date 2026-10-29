using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class EventService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
    /// <summary>Bir etkinliğe en fazla bu kadar fotoğraf eklenebilir.</summary>
    private const int MaxPhotos = 12;

    public async Task<List<EventListItemDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var events = await dbContext.Events
            .AsNoTracking()
            .Include(e => e.Photos)
            .OrderBy(e => e.SortOrder)
            .ToListAsync(cancellationToken);

        return events.Select(ToListItem).ToList();
    }

    /// <summary>Yeni etkinlik listenin sonuna eklenir. Fotoğraflar ayrıca yüklenir.</summary>
    public async Task<EventListItemDto> CreateAsync(EventCommandDto request, CancellationToken cancellationToken)
    {
        var item = new Event { SortOrder = await Ordering.NextAsync(dbContext.Events, cancellationToken) };
        Fill(item, request);

        dbContext.Events.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToListItem(item);
    }

    public async Task UpdateAsync(Guid id, EventCommandDto request, CancellationToken cancellationToken)
    {
        var item = await FindAsync(id, cancellationToken);
        Fill(item, request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Etkinliği fotoğraflarıyla birlikte siler.</summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await FindAsync(id, cancellationToken);
        dbContext.Events.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var photo in item.Photos)
            imageStorage.Delete(ImageStorageProvider.Site, photo.FileName);
    }

    public async Task ReorderAsync(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await Ordering.ApplyAsync(dbContext.Events, request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Fotoğrafı etkinliğin fotoğraflarının sonuna ekler. Etkinliğin güncel hâlini döndürür.</summary>
    public async Task<EventListItemDto> AddPhotoAsync(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        var item = await FindAsync(id, cancellationToken);
        if (item.Photos.Count >= MaxPhotos)
            throw Errors.BadRequest($"Bir etkinliğe en fazla {MaxPhotos} fotoğraf eklenebilir.");

        // Açıkça eklenir: ID'si hazır gelen kayıt navigasyona eklenirse EF onu var olan kayıt sanar.
        dbContext.EventPhotos.Add(new EventPhoto
        {
            EventId = item.Id,
            FileName = await imageStorage.SaveNewAsync(ImageStorageProvider.Site, file, cancellationToken),
            SortOrder = await Ordering.NextAsync(dbContext.EventPhotos.Where(p => p.EventId == id), cancellationToken)
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToListItem(item);
    }

    public async Task DeletePhotoAsync(Guid id, Guid photoId, CancellationToken cancellationToken)
    {
        var photo = await dbContext.EventPhotos.FirstOrDefaultAsync(p => p.Id == photoId && p.EventId == id, cancellationToken)
            ?? throw Errors.NotFound("Fotoğraf bulunamadı.");

        dbContext.EventPhotos.Remove(photo);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, photo.FileName);
    }

    /// <summary>Fotoğrafların sırasını değiştirir; ilk sıradaki kapak olur.</summary>
    public async Task ReorderPhotosAsync(Guid id, OrderCommandDto request, CancellationToken cancellationToken)
    {
        await FindAsync(id, cancellationToken);
        await Ordering.ApplyAsync(dbContext.EventPhotos.Where(p => p.EventId == id), request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Fill(Event item, EventCommandDto request)
    {
        var style = request.Style ?? EventStyle.Wall;
        if (!Enum.IsDefined(style))
            throw Errors.BadRequest("Görünüm geçerli değil.");

        item.Title = Input.RequiredText(request.Title, "Başlık", 2, 100);
        item.Description = Input.OptionalText(request.Description, "Açıklama", 1000);
        item.Style = style;
        item.IsVisible = request.IsVisible ?? true;
    }

    private static EventListItemDto ToListItem(Event item) =>
        new(
            item.Id,
            item.Title,
            item.Description,
            item.Style,
            item.Photos.OrderBy(p => p.SortOrder).Select(p => new EventPhotoDto(p.Id, p.FileName)).ToList(),
            item.IsVisible);

    private async Task<Event> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Events.Include(e => e.Photos).FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Etkinlik bulunamadı.");
}
