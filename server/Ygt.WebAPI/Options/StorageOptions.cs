namespace Ygt.WebAPI.Options;

/// <summary>
/// Yüklenen dosyaların (sertifika görselleri) saklandığı klasör.
/// Göreli yol uygulama klasörüne göre çözülür. Docker'da bu klasör bir volume'dür.
/// </summary>
public sealed class StorageOptions
{
    public string RootPath { get; set; } = "storage";
}
