using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Extensions;
using Ygt.WebAPI.Services;

namespace Ygt.WebAPI.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    public async Task<IActionResult> Login(LoginDto request, CancellationToken cancellationToken) =>
        Ok(await service.LoginAsync(request, cancellationToken));
}
