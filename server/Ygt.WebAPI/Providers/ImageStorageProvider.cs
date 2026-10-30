using Microsoft.Extensions.Options;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Options;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Providers;

/// <summary>
/// Yüklenen görselleri storage klasörünün alt klasörlerinde saklar: sertifika görselleri ve sitenin görselleri
/// (logo, sekme simgesi, giriş görseli, ekip fotoğrafları, kart görselleri).
/// Docker'da storage klasörü bir volume'dür. Veritabanında sadece dosya adı tutulur.
/// </summary>
public sealed class ImageStorageProvider(IOptions<StorageOptions> storageOptions, IWebHostEnvironment environment)
{
    public const string Certificates = "certificates";

    /// <summary>Sitenin görselleri. Hepsi herkese açıktır; GET /site/content/images/{dosya} ile verilir.</summary>
    public const string Site = "site";

    private const long MaxImageSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".webp"] = "image/webp"
    };

    /// <summary>
    /// Görseli doğrular ve klasöre "{name}.{uzantı}" adıyla kaydeder. Aynı adda dosya varsa üzerine yazılır.
    /// Kaydedilen dosya adını döndürür.
    /// </summary>
    public async Task<string> SaveAsync(string folder, string name, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            throw Errors.BadRequest("Görsel dosyası seçilmelidir.");

        if (file.Length > MaxImageSize)
            throw Errors.BadRequest("Görsel en fazla 5 MB olabilir.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!ContentTypes.ContainsKey(extension))
            throw Errors.BadRequest("Sadece PNG, JPG veya WEBP görsel yüklenebilir.");

        var fileName = $"{name}{extension}";
        Directory.CreateDirectory(DirectoryOf(folder));

        await using var stream = File.Create(PathOf(folder, fileName));
        await file.CopyToAsync(stream, cancellationToken);

        return fileName;
    }

    /// <summary>
    /// Görseli yeni ve tekil bir adla kaydeder. Görsel değiştiğinde adı da değiştiği için tarayıcı önbelleğindeki
    /// eski görsel yerine yenisi görünür. Kaydedilen dosya adını döndürür.
    /// </summary>
    public Task<string> SaveNewAsync(string folder, IFormFile? file, CancellationToken cancellationToken) =>
        SaveAsync(folder, Guid.NewGuid().ToString(), file, cancellationToken);

    /// <summary>
    /// Diskteki bir görseli (ör. SeedData'daki başlangıç görseli) klasöre yeni ve tekil bir adla kopyalar.
    /// Böylece panelden yüklenen görsellerle aynı şekilde değiştirilip silinebilir. Kaydedilen dosya adını döndürür.
    /// </summary>
    public string CopyNew(string folder, string sourcePath)
    {
        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!ContentTypes.ContainsKey(extension))
            throw new InvalidOperationException($"Desteklenmeyen görsel türü: {sourcePath}");

        var fileName = $"{Guid.NewGuid()}{extension}";
        Directory.CreateDirectory(DirectoryOf(folder));
        File.Copy(sourcePath, PathOf(folder, fileName));

        return fileName;
    }

    public async Task<FileDto> ReadAsync(string folder, string? fileName, string notFoundMessage, CancellationToken cancellationToken)
    {
        var path = fileName is null ? null : PathOf(folder, fileName);
        if (path is null || !File.Exists(path) || !ContentTypes.TryGetValue(Path.GetExtension(path), out var contentType))
            throw Errors.NotFound(notFoundMessage);

        var content = await File.ReadAllBytesAsync(path, cancellationToken);
        return new FileDto(content, contentType, fileName!);
    }

    /// <summary>Dosyayı siler. Ad boşsa (görsel hiç yüklenmemişse) bir şey yapmaz.</summary>
    public void Delete(string folder, string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        var path = PathOf(folder, fileName);
        if (File.Exists(path))
            File.Delete(path);
    }

    private string DirectoryOf(string folder) =>
        Path.Combine(environment.ContentRootPath, storageOptions.Value.RootPath, folder);

    // Dosya adları veritabanından gelir ve API tarafından üretilir; yine de klasör dışına çıkılamasın diye
    // yol ayırıcıları atılıp sadece ad kısmı kullanılır.
    private string PathOf(string folder, string fileName) =>
        Path.Combine(DirectoryOf(folder), Path.GetFileName(fileName));
}
