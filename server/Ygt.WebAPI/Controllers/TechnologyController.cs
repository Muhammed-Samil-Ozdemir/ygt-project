using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>Ana sayfadaki teknoloji etiketleri. Sitede görünen liste GET /site/content ile gelir; buradaki uçlar admin içindir.</summary>
[Route("site/content/technologies")]
[ApiController]
[Authorize]
public class TechnologyController(TechnologyService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(TechnologyCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, TechnologyCommandDto request, CancellationToken cancellationToken)
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
}
