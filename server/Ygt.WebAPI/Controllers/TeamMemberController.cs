using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>Ana sayfadaki ekip listesi. Sitede görünen liste GET /site/content ile gelir; buradaki uçlar admin içindir.</summary>
[Route("site/content/team")]
[ApiController]
[Authorize]
public class TeamMemberController(TeamMemberService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(TeamMemberCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, TeamMemberCommandDto request, CancellationToken cancellationToken)
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

    /// <summary>Fotoğraf multipart/form-data ile "file" alanında gönderilir. Kişinin güncel hâli döner.</summary>
    [HttpPut("{id:guid}/photo")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile? file, CancellationToken cancellationToken) =>
        Ok(await service.UploadPhotoAsync(id, file, cancellationToken));

    [HttpDelete("{id:guid}/photo")]
    public async Task<IActionResult> DeletePhoto(Guid id, CancellationToken cancellationToken)
    {
        await service.DeletePhotoAsync(id, cancellationToken);
        return Ok();
    }

    [HttpPut("order")]
    public async Task<IActionResult> Reorder(OrderCommandDto request, CancellationToken cancellationToken)
    {
        await service.ReorderAsync(request, cancellationToken);
        return Ok();
    }
}
