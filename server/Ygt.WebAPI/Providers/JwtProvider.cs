using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Options;

namespace Ygt.WebAPI.Providers;

public sealed class JwtProvider(IOptions<JwtOptions> options)
{
    public TokenDto CreateToken(User user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(options.Value.ExpiresInHours);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(options.Value.SecretKey));

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            ]),
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new TokenDto(token, expiresAt);
    }
}
