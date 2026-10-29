using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Extensions;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

[Route("courses")]
[ApiController]
public class CourseController(CourseService service) : ControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken) =>
        Ok(await service.GetActiveAsync(cancellationToken));

    [HttpPost("{id:guid}/applications")]
    [EnableRateLimiting(RateLimitPolicies.Applications)]
    public async Task<IActionResult> Apply(Guid id, CourseApplicationCommandDto request, CancellationToken cancellationToken)
    {
        await service.ApplyAsync(id, request, cancellationToken);
        return Ok(new MessageDto("Başvurunuz alındı. Teşekkür ederiz."));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CourseCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, CourseCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost("{id:guid}/close")]
    [Authorize]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(id, false, cancellationToken);
        return Ok();
    }

    [HttpPost("{id:guid}/reopen")]
    [Authorize]
    public async Task<IActionResult> Reopen(Guid id, CancellationToken cancellationToken)
    {
        await service.SetActiveAsync(id, true, cancellationToken);
        return Ok();
    }

    [HttpGet("{id:guid}/applications")]
    [Authorize]
    public async Task<IActionResult> GetApplications(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetApplicationsAsync(id, cancellationToken));

    [HttpDelete("{id:guid}/applications")]
    [Authorize]
    public async Task<IActionResult> DeleteApplications(Guid id, CancellationToken cancellationToken)
    {
        var count = await service.DeleteApplicationsAsync(id, cancellationToken);
        return Ok(new MessageDto($"{count} başvuru silindi."));
    }

    [HttpDelete("{id:guid}/applications/{applicationId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteApplication(Guid id, Guid applicationId, CancellationToken cancellationToken)
    {
        await service.DeleteApplicationAsync(id, applicationId, cancellationToken);
        return Ok();
    }

    [HttpGet("{id:guid}/applications/export")]
    [Authorize]
    public async Task<IActionResult> ExportApplications(Guid id, CancellationToken cancellationToken)
    {
        var file = await service.ExportApplicationsAsync(id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
