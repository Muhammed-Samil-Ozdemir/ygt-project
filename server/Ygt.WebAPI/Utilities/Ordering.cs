using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Utilities;

/// <summary>Panelden sıralanan listeler (ekip, hakkımızda kartları, teknolojiler, bağlantılar) için ortak işlemler.</summary>
public static class Ordering
{
    /// <summary>Listenin sonuna eklenecek kaydın sırası.</summary>
    public static async Task<int> NextAsync<T>(IQueryable<T> items, CancellationToken cancellationToken)
        where T : class, ISortable =>
        (await items.MaxAsync(x => (int?)x.SortOrder, cancellationToken) ?? -1) + 1;

    /// <summary>
    /// Kayıtları verilen ID sırasına dizer; kaydetmek çağırana kalır. Listede olmayan kayıtlar
    /// (ör. bu arada başka sekmede eklenen) kendi aralarındaki sırayla sona geçer.
    /// </summary>
    public static async Task ApplyAsync<T>(IQueryable<T> items, List<Guid>? ids, CancellationToken cancellationToken)
        where T : class, ISortable
    {
        if (ids is null || ids.Count == 0)
            throw Errors.BadRequest("Sıralama için kayıt listesi gönderilmelidir.");

        var current = await items.OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

        var ordered = current
            .OrderBy(x => ids.IndexOf(x.Id) is var index and >= 0 ? index : int.MaxValue)
            .ToList();

        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].SortOrder = i;
        }
    }
}
