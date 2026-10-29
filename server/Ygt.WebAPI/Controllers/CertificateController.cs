using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

[Route("certificates")]
[ApiController]
public class CertificateController(CertificateService service) : ControllerBase
{
    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken) =>
        Ok(await service.GetByCodeAsync(code, cancellationToken));

    [HttpGet("{code}/image")]
    public async Task<IActionResult> GetImage(string code, CancellationToken cancellationToken)
    {
        var file = await service.GetImageAsync(code, cancellationToken);
        return File(file.Content, file.ContentType);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    /// <summary>Bütün sertifikalar doğrulama linkleriyle Excel olarak. siteUrl: linklerin başındaki site adresi.</summary>
    [HttpGet("export")]
    [Authorize]
    public async Task<IActionResult> Export([FromQuery] string? siteUrl, CancellationToken cancellationToken)
    {
        var file = await service.ExportAsync(siteUrl, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    /// <summary>Toplu ekleme için boş Excel şablonu.</summary>
    [HttpGet("import/template")]
    [Authorize]
    public IActionResult GetImportTemplate()
    {
        var file = service.GetImportTemplate();
        return File(file.Content, file.ContentType, file.FileName);
    }

    /// <summary>Excel'den toplu ekleme; multipart/form-data, "file" alanı. Hatalı satır varsa hiçbiri eklenmez.</summary>
    [HttpPost("import")]
    [Authorize]
    public async Task<IActionResult> Import(IFormFile? file, CancellationToken cancellationToken) =>
        Ok(await service.ImportAsync(file, cancellationToken));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CertificateCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, CertificateCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    /// <summary>Görsel multipart/form-data ile "file" alanında gönderilir.</summary>
    [HttpPut("{id:guid}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        await service.UploadImageAsync(id, file, cancellationToken);
        return Ok();
    }
}
