using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>
/// Sitenin içeriği. Listeler (ekip, hakkımızda, teknolojiler, bağlantılar) kendi controller'larında,
/// "site/content/..." altında yönetilir.
/// </summary>
[Route("site/content")]
[ApiController]
public class SiteContentController(SiteContentService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(cancellationToken));

    /// <summary>Ekip fotoğrafı. Dosya adı her yüklemede değiştiği için tarayıcı uzun süre önbellekte tutabilir.</summary>
    [HttpGet("photos/{fileName}")]
    [ResponseCache(Duration = 60 * 60 * 24 * 365)]
    public async Task<IActionResult> GetPhoto(string fileName, CancellationToken cancellationToken)
    {
        var file = await service.GetPhotoAsync(fileName, cancellationToken);
        return File(file.Content, file.ContentType);
    }

    [HttpGet("texts")]
    [Authorize]
    public async Task<IActionResult> GetTexts(CancellationToken cancellationToken) =>
        Ok(await service.GetTextsAsync(cancellationToken));

    [HttpPut("texts")]
    [Authorize]
    public async Task<IActionResult> UpdateTexts(SiteTextsCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateTextsAsync(request, cancellationToken);
        return Ok();
    }
}
