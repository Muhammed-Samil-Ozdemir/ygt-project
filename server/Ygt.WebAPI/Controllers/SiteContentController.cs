using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>
/// Sitenin içeriği. Tek parça metinler ve görseller (logo, sekme simgesi...) burada; listeler (ekip, hakkımızda,
/// teknolojiler, bağlantılar) kendi controller'larında, "site/content/..." altında yönetilir.
/// </summary>
[Route("site/content")]
[ApiController]
public class SiteContentController(SiteContentService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(cancellationToken));

    /// <summary>
    /// Sitenin görseli (logo, ekip fotoğrafı, kart görseli...). Dosya adı her yüklemede değiştiği için tarayıcı
    /// uzun süre önbellekte tutabilir.
    /// </summary>
    [HttpGet("images/{fileName}")]
    [ResponseCache(Duration = 60 * 60 * 24 * 365)]
    public async Task<IActionResult> GetImage(string fileName, CancellationToken cancellationToken)
    {
        var file = await service.GetImageAsync(fileName, cancellationToken);
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

    /// <summary>Görsel alanına (ör. "logo") görsel yükler; multipart/form-data, "file" alanı. Alanın güncel hâli döner.</summary>
    [HttpPut("texts/{key}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(string key, IFormFile? file, CancellationToken cancellationToken) =>
        Ok(await service.UploadImageAsync(key, file, cancellationToken));

    [HttpDelete("texts/{key}/image")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(string key, CancellationToken cancellationToken)
    {
        await service.DeleteImageAsync(key, cancellationToken);
        return Ok();
    }
}
