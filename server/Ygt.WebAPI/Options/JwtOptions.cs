namespace Ygt.WebAPI.Options;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;

    /// <summary>HMAC-SHA512 imzalama anahtarı, en az 64 karakter. Koda yazılmaz, ortam değişkeninden gelir.</summary>
    public string SecretKey { get; set; } = default!;

    public int ExpiresInHours { get; set; } = 4;
}
