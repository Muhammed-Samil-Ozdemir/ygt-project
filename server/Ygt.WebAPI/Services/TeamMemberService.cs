using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class TeamMemberService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
    public async Task<List<TeamMemberListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.TeamMembers
            .OrderBy(m => m.SortOrder)
            .Select(m => new TeamMemberListItemDto(m.Id, m.Name, m.Role, m.PhotoFileName, m.IsVisible))
            .ToListAsync(cancellationToken);

    /// <summary>Yeni kişi listenin sonuna eklenir. Fotoğraf ayrıca yüklenir.</summary>
    public async Task<TeamMemberListItemDto> CreateAsync(TeamMemberCommandDto request, CancellationToken cancellationToken)
    {
        var member = new TeamMember { SortOrder = await Ordering.NextAsync(dbContext.TeamMembers, cancellationToken) };
        Fill(member, request);

        dbContext.TeamMembers.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToListItem(member);
    }

    public async Task UpdateAsync(Guid id, TeamMemberCommandDto request, CancellationToken cancellationToken)
    {
        var member = await FindAsync(id, cancellationToken);
        Fill(member, request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await FindAsync(id, cancellationToken);

        dbContext.TeamMembers.Remove(member);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, member.PhotoFileName);
    }

    /// <summary>Fotoğrafı yükler, öncekini siler. Kişinin güncel hâlini döndürür.</summary>
    public async Task<TeamMemberListItemDto> UploadPhotoAsync(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        var member = await FindAsync(id, cancellationToken);
        var oldFileName = member.PhotoFileName;

        member.PhotoFileName = await imageStorage.SaveNewAsync(ImageStorageProvider.Site, file, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);

        return ToListItem(member);
    }

    public async Task DeletePhotoAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await FindAsync(id, cancellationToken);
        var oldFileName = member.PhotoFileName;

        member.PhotoFileName = null;
        await dbContext.SaveChangesAsync(cancellationToken);

        imageStorage.Delete(ImageStorageProvider.Site, oldFileName);
    }

    public async Task ReorderAsync(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await Ordering.ApplyAsync(dbContext.TeamMembers, request.Ids, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Fill(TeamMember member, TeamMemberCommandDto request)
    {
        member.Name = Input.RequiredText(request.Name, "Ad soyad", 2, 100);
        member.Role = Input.RequiredText(request.Role, "Görev", 2, 100);
        member.IsVisible = request.IsVisible ?? true;
    }

    private static TeamMemberListItemDto ToListItem(TeamMember member) =>
        new(member.Id, member.Name, member.Role, member.PhotoFileName, member.IsVisible);

    private async Task<TeamMember> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.TeamMembers.FirstOrDefaultAsync(m => m.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Ekip üyesi bulunamadı.");
}
