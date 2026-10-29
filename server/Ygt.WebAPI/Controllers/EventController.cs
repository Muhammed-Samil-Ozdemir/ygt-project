using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>Ana sayfadaki etkinlikler ve fotoğrafları. Sitede görünen liste GET /site/content ile gelir; buradaki uçlar admin içindir.</summary>
[Route("site/content/events")]
[ApiController]
[Authorize]
public class EventController(EventService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(EventCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, EventCommandDto request, CancellationToken cancellationToken)
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

    [HttpPut("order")]
    public async Task<IActionResult> Reorder(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await service.ReorderAsync(request, cancellationToken);
        return Ok();
    }

    /// <summary>Fotoğraf multipart/form-data ile "file" alanında gönderilir. Etkinliğin güncel hâli döner.</summary>
    [HttpPost("{id:guid}/photos")]
    public async Task<IActionResult> AddPhoto(Guid id, IFormFile? file, CancellationToken cancellationToken) =>
        Ok(await service.AddPhotoAsync(id, file, cancellationToken));

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId, CancellationToken cancellationToken)
    {
        await service.DeletePhotoAsync(id, photoId, cancellationToken);
        return Ok();
    }

    [HttpPut("{id:guid}/photos/order")]
    public async Task<IActionResult> ReorderPhotos(Guid id, OrderCommandDto request, CancellationToken cancellationToken)
    {
        await service.ReorderPhotosAsync(id, request, cancellationToken);
        return Ok();
    }
}
