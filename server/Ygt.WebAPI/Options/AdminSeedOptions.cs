namespace Ygt.WebAPI.Options;

/// <summary>
/// "Seed:Admin" ayarları. Şifre koda yazılmaz: yerelde user-secrets'tan, Docker'da .env dosyasından gelir.
/// </summary>
public sealed class AdminSeedOptions
{
    public string Name { get; set; } = "Admin";
    public string Email { get; set; } = "admin@ygt.com";
    public string? Password { get; set; }
}
