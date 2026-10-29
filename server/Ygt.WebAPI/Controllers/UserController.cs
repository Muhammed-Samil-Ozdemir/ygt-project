using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Extensions;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

/// <summary>Panel yöneticileri. Yöneticinin kendi bilgileri ve şifresi /auth altında.</summary>
[Route("users")]
[ApiController]
[Authorize]
public class UserController(UserService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(User.GetUserId(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(UserCommandDto request, CancellationToken cancellationToken) =>
        Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UserUpdateCommandDto request, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> SetPassword(Guid id, PasswordCommandDto request, CancellationToken cancellationToken)
    {
        await service.SetPasswordAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return Ok();
    }
}
