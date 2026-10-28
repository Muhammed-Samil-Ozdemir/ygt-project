using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class AuthService(AppDbContext dbContext, JwtProvider jwtProvider)
{
    public async Task<TokenDto> LoginAsync(LoginDto request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(request.Password))
            throw Errors.BadRequest("E-posta ve şifre zorunludur.");

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);

        // Kullanıcının var olup olmadığı belli edilmez, iki durumda da aynı mesaj dönülür.
        if (user is null || !user.VerifyPassword(request.Password))
            throw Errors.Unauthorized("E-posta veya şifre hatalı.");

        return jwtProvider.CreateToken(user);
    }
}
