using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Extensions;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(AuthService service, UserService userService) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Login(LoginDto request, CancellationToken cancellationToken) =>
        Ok(await service.LoginAsync(request, cancellationToken));

    /// <summary>Giriş yapmış yöneticinin bilgileri.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken) =>
        Ok(await userService.GetCurrentAsync(User.GetUserId(), cancellationToken));

    /// <summary>Kendi şifresini değiştirir. Mevcut şifre denendiği için giriş gibi istek sınırına tabidir.</summary>
    [HttpPut("password")]
    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommandDto request, CancellationToken cancellationToken)
    {
        await userService.ChangeOwnPasswordAsync(User.GetUserId(), request, cancellationToken);
        return Ok(new MessageDto("Şifreniz değiştirildi."));
    }
}
