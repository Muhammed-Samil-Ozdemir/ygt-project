using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>Ana sayfadaki hakkımızda kartları. Sitede görünen liste GET /site/content ile gelir; buradaki uçlar admin içindir.</summary>
[Route("site/content/about")]
[ApiController]
[Authorize]
public class AboutCardController(AboutCardService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(AboutCardCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, AboutCardCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    /// <summary>Görsel multipart/form-data ile "file" alanında gönderilir. Kartın güncel hâli döner.</summary>
    [HttpPut("{id:guid}/image")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile? file, CancellationToken cancellationToken) =>
        Ok(await service.UploadImageAsync(id, file, cancellationToken));

    [HttpDelete("{id:guid}/image")]
    public async Task<IActionResult> DeleteImage(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteImageAsync(id, cancellationToken);
        return Ok();
    }

    [HttpPut("order")]
    public async Task<IActionResult> Reorder(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await service.ReorderAsync(request, cancellationToken);
        return Ok();
    }
}
