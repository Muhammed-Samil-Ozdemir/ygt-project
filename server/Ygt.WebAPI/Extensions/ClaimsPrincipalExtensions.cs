using System.Security.Claims;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Token'daki yönetici ID'si. [Authorize] olan uçlarda her zaman bulunur.</summary>
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw Errors.Unauthorized("Oturum bilgisi okunamadı. Lütfen tekrar giriş yapın.");
}
