using Ygt.WebAPI.Abstractions;

namespace Ygt.WebAPI.Models;

/// <summary>Admin paneline giriş yapabilen kullanıcı.</summary>
public sealed class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;

    /// <summary>bcrypt özeti. Eski sistemdeki özetler de olduğu gibi çalışır.</summary>
    public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public bool VerifyPassword(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}
