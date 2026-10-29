using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;

namespace Ygt.WebAPI.Options;

public sealed class JwtSetupOptions(IOptions<JwtOptions> jwtOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);

        options.TokenValidationParameters.ValidIssuer = jwtOptions.Value.Issuer;
        options.TokenValidationParameters.ValidAudience = jwtOptions.Value.Audience;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        options.TokenValidationParameters.IssuerSigningKey = signingKey;

        options.Events = new JwtBearerEvents
        {
            // Token geçerli olsa bile yönetici silinmiş ya da pasifse istek reddedilir;
            // silinen yöneticinin elindeki token süresi dolmadan da işe yaramaz.
            OnTokenValidated = async context =>
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var exists = Guid.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                    && await dbContext.Users.AnyAsync(u => u.Id == userId && u.IsActive, context.HttpContext.RequestAborted);

                if (!exists)
                    context.Fail("Yönetici bulunamadı.");
            },

            // Yetkisiz isteklerde de diğer hatalar gibi { message } gövdesi dönülür.
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    new MessageDto("Oturumunuz bulunamadı veya süresi doldu. Lütfen tekrar giriş yapın."));
            }
        };
    }
}
