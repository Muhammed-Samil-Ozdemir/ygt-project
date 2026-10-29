namespace Ygt.WebAPI.Options;

/// <summary>
/// "Seed:Admin" ayarları: ilk kurulumda oluşturulan yönetici. Şifre koda yazılmaz: yerelde user-secrets'tan,
/// Docker'da .env dosyasından gelir.
/// </summary>
public sealed class AdminSeedOptions
{
    public string Name { get; set; } = "Admin";
    public string Email { get; set; } = "admin@ygt.com";
    public string? Password { get; set; }

    /// <summary>
    /// true ise açılışta bu yöneticinin şifresi Password'e döndürülür (şifre unutulduğunda).
    /// İş bitince false yapılmalı; yoksa panelden değiştirilen şifre her açılışta geri alınır.
    /// </summary>
    public bool ResetPassword { get; set; }
}
