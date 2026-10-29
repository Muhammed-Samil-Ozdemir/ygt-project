using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class SiteLinkService(AppDbContext dbContext)
{
    public async Task<List<SiteLinkListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.SiteLinks
            .OrderBy(l => l.SortOrder)
            .Select(l => new SiteLinkListItemDto(l.Id, l.Label, l.Url, l.IsVisible))
            .ToListAsync(cancellationToken);

    /// <summary>Yeni bağlantı listenin sonuna eklenir.</summary>
    public async Task<SiteLinkListItemDto> CreateAsync(SiteLinkCommandDto request, CancellationToken cancellationToken)
    {
        var link = new SiteLink { SortOrder = await Ordering.NextAsync(dbContext.SiteLinks, cancellationToken) };
        Fill(link, request);

        dbContext.SiteLinks.Add(link);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SiteLinkListItemDto(link.Id, link.Label, link.Url, link.IsVisible);
    }

    public async Task UpdateAsync(Guid id, SiteLinkCommandDto request, CancellationToken cancellationToken)
    {
        var link = await FindAsync(id, cancellationToken);
        Fill(link, request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var link = await FindAsync(id, cancellationToken);
        dbContext.SiteLinks.Remove(link);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReorderAsync(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await Ordering.ApplyAsync(dbContext.SiteLinks, request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Fill(SiteLink link, SiteLinkCommandDto request)
    {
        link.Label = Input.RequiredText(request.Label, "Bağlantı adı", 2, 50);
        link.Url = Input.RequiredUrl(request.Url, "Bağlantı adresi");
        link.IsVisible = request.IsVisible ?? true;
    }

    private async Task<SiteLink> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.SiteLinks.FirstOrDefaultAsync(l => l.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Bağlantı bulunamadı.");
}
