using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

/// <summary>Panel yöneticileri. Bütün yöneticilerin yetkisi aynıdır.</summary>
public sealed class UserService(AppDbContext dbContext)
{
    public async Task<List<UserListItemDto>> GetAllAsync(Guid currentUserId, CancellationToken cancellationToken) =>
        await dbContext.Users
            .OrderBy(u => u.CreatedAt)
            .Select(u => new UserListItemDto(u.Id, u.Name, u.Email, u.CreatedAt, u.Id == currentUserId))
            .ToListAsync(cancellationToken);

    public async Task<CurrentUserDto> GetCurrentAsync(Guid currentUserId, CancellationToken cancellationToken) =>
        await dbContext.Users
            .Where(u => u.Id == currentUserId)
            .Select(u => new CurrentUserDto(u.Id, u.Name, u.Email))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw Errors.NotFound("Yönetici bulunamadı.");

    public async Task<UserListItemDto> CreateAsync(UserCommandDto request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = Input.RequiredText(request.Name, "Ad soyad", 2, 100),
            Email = Input.RequiredEmail(request.Email, "E-posta"),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password(request.Password, "Şifre"))
        };

        await EnsureEmailFreeAsync(user.Email, null, cancellationToken);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserListItemDto(user.Id, user.Name, user.Email, user.CreatedAt, false);
    }

    public async Task UpdateAsync(Guid id, UserUpdateCommandDto request, CancellationToken cancellationToken)
    {
        var user = await FindAsync(id, cancellationToken);
        var email = Input.RequiredEmail(request.Email, "E-posta");

        await EnsureEmailFreeAsync(email, id, cancellationToken);

        user.Name = Input.RequiredText(request.Name, "Ad soyad", 2, 100);
        user.Email = email;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Başka bir yöneticiye yeni şifre belirler, ör. şifresini unuttuysa.</summary>
    public async Task SetPasswordAsync(Guid id, PasswordCommandDto request, CancellationToken cancellationToken)
    {
        var user = await FindAsync(id, cancellationToken);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password(request.Password, "Şifre"));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Yöneticinin kendi şifresini değiştirir; önce mevcut şifresi doğrulanır.</summary>
    public async Task ChangeOwnPasswordAsync(
        Guid currentUserId,
        ChangePasswordCommandDto request,
        CancellationToken cancellationToken)
    {
        var user = await FindAsync(currentUserId, cancellationToken);

        if (string.IsNullOrEmpty(request.CurrentPassword) || !user.VerifyPassword(request.CurrentPassword))
            throw Errors.BadRequest("Mevcut şifre hatalı.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password(request.NewPassword, "Yeni şifre"));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Yöneticiyi siler. Kişi kendini silemez; böylece panelde her zaman en az bir yönetici kalır.
    /// Silinen yöneticinin elindeki token da hemen geçersiz olur (JwtSetupOptions).
    /// </summary>
    public async Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken)
    {
        if (id == currentUserId)
            throw Errors.BadRequest("Kendi hesabınızı silemezsiniz.");

        var user = await FindAsync(id, cancellationToken);

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureEmailFreeAsync(string email, Guid? exceptId, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == email && u.Id != exceptId, cancellationToken))
            throw Errors.Conflict("Bu e-posta adresiyle kayıtlı bir yönetici zaten var.");
    }

    private async Task<User> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Yönetici bulunamadı.");
}
