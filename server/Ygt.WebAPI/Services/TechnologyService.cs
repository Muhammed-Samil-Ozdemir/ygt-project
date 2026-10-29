using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class TechnologyService(AppDbContext dbContext)
{
    public async Task<List<TechnologyListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Technologies
            .OrderBy(t => t.SortOrder)
            .Select(t => new TechnologyListItemDto(t.Id, t.Name))
            .ToListAsync(cancellationToken);

    /// <summary>Yeni teknoloji listenin sonuna eklenir.</summary>
    public async Task<TechnologyListItemDto> CreateAsync(TechnologyCommandDto request, CancellationToken cancellationToken)
    {
        var name = await ReadNameAsync(request, null, cancellationToken);

        var technology = new Technology
        {
            Name = name,
            SortOrder = await Ordering.NextAsync(dbContext.Technologies, cancellationToken)
        };

        dbContext.Technologies.Add(technology);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TechnologyListItemDto(technology.Id, technology.Name);
    }

    public async Task UpdateAsync(Guid id, TechnologyCommandDto request, CancellationToken cancellationToken)
    {
        var technology = await FindAsync(id, cancellationToken);
        technology.Name = await ReadNameAsync(request, id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var technology = await FindAsync(id, cancellationToken);
        dbContext.Technologies.Remove(technology);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReorderAsync(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await Ordering.ApplyAsync(dbContext.Technologies, request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Aynı teknoloji iki kez eklenemez (veritabanı harf büyüklüğüne bakmadan karşılaştırır).</summary>
    private async Task<string> ReadNameAsync(TechnologyCommandDto request, Guid? id, CancellationToken cancellationToken)
    {
        var name = Input.RequiredText(request.Name, "Teknoloji adı", 1, 50);

        if (await dbContext.Technologies.AnyAsync(t => t.Name == name && t.Id != id, cancellationToken))
            throw Errors.Conflict($"\"{name}\" listede zaten var.");

        return name;
    }

    private async Task<Technology> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Technologies.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Teknoloji bulunamadı.");
}
