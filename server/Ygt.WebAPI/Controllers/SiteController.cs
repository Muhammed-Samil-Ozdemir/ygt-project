using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

[Route("site")]
[ApiController]
public class SiteController(SiteService service) : ControllerBase
{
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken) =>
        Ok(await service.GetSettingsAsync(cancellationToken));

    [HttpPut("settings")]
    [Authorize]
    public async Task<IActionResult> UpdateSettings(SiteSettingsCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateSettingsAsync(request, cancellationToken);
        return Ok();
    }

    [HttpPost("visits/{page}")]
    public async Task<IActionResult> RecordVisit(string page, CancellationToken cancellationToken)
    {
        await service.RecordVisitAsync(page, cancellationToken);
        return NoContent();
    }

    [HttpGet("visits")]
    [Authorize]
    public async Task<IActionResult> GetVisits(CancellationToken cancellationToken) =>
        Ok(await service.GetVisitsAsync(cancellationToken));
}
