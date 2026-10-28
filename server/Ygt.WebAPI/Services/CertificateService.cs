using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Options;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class CertificateService(
    AppDbContext dbContext,
    IOptions<StorageOptions> storageOptions,
    IWebHostEnvironment environment)
{
    private const long MaxImageSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> ImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".webp"] = "image/webp"
    };

    private string ImageDirectory =>
        Path.Combine(environment.ContentRootPath, storageOptions.Value.RootPath, "certificates");

    public async Task<CertificateDto> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        await dbContext.Certificates
            .Where(c => c.Code == code)
            .Select(c => new CertificateDto(c.Code, c.CourseName, c.FirstName, c.LastName, c.ImageFileName != null))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw Errors.NotFound("Sertifika bulunamadı.");

    public async Task<FileDto> GetImageAsync(string code, CancellationToken cancellationToken)
    {
        var fileName = await dbContext.Certificates
            .Where(c => c.Code == code)
            .Select(c => c.ImageFileName)
            .FirstOrDefaultAsync(cancellationToken);

        var path = fileName is null ? null : Path.Combine(ImageDirectory, fileName);
        if (path is null || !File.Exists(path))
            throw Errors.NotFound("Sertifika görseli bulunamadı.");

        var content = await File.ReadAllBytesAsync(path, cancellationToken);
        return new FileDto(content, ImageContentTypes[Path.GetExtension(path)], fileName!);
    }

    public async Task<List<CertificateListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Certificates
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CertificateListItemDto(
                c.Id,
                c.Code,
                c.CourseName,
                c.FirstName,
                c.LastName,
                c.Email,
                c.PhoneNumber,
                c.ImageFileName != null,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Sertifikayı oluşturur ve rastgele bir doğrulama kodu verir. Kod tahmin edilemez olsun diye
    /// zamana dayalı GUID değil, rastgele GUID kullanılır.
    /// </summary>
    public async Task<CertificateListItemDto> CreateAsync(CertificateCommandDto request, CancellationToken cancellationToken)
    {
        var certificate = new Certificate { Code = Guid.NewGuid().ToString() };
        Fill(certificate, request);

        dbContext.Certificates.Add(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CertificateListItemDto(
            certificate.Id,
            certificate.Code,
            certificate.CourseName,
            certificate.FirstName,
            certificate.LastName,
            certificate.Email,
            certificate.PhoneNumber,
            false,
            certificate.CreatedAt);
    }

    public async Task UpdateAsync(Guid id, CertificateCommandDto request, CancellationToken cancellationToken)
    {
        var certificate = await FindAsync(id, cancellationToken);
        Fill(certificate, request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var certificate = await FindAsync(id, cancellationToken);

        dbContext.Certificates.Remove(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);

        DeleteImageFile(certificate.ImageFileName);
    }

    /// <summary>Görseli sertifika ID'siyle adlandırıp kaydeder. Önceki görsel varsa yerine geçer.</summary>
    public async Task UploadImageAsync(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            throw Errors.BadRequest("Görsel dosyası seçilmelidir.");

        if (file.Length > MaxImageSize)
            throw Errors.BadRequest("Görsel en fazla 5 MB olabilir.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!ImageContentTypes.ContainsKey(extension))
            throw Errors.BadRequest("Sadece PNG, JPG veya WEBP görsel yüklenebilir.");

        var certificate = await FindAsync(id, cancellationToken);

        Directory.CreateDirectory(ImageDirectory);
        var fileName = $"{certificate.Id}{extension}";

        await using (var stream = File.Create(Path.Combine(ImageDirectory, fileName)))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        if (certificate.ImageFileName != fileName)
            DeleteImageFile(certificate.ImageFileName);

        certificate.ImageFileName = fileName;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Fill(Certificate certificate, CertificateCommandDto request)
    {
        certificate.CourseName = Input.RequiredText(request.CourseName, "Eğitim adı", 3, 255);
        certificate.FirstName = Input.RequiredText(request.FirstName, "Ad", 2, 100);
        certificate.LastName = Input.RequiredText(request.LastName, "Soyad", 2, 100);
        certificate.Email = Input.OptionalEmail(request.Email, "E-posta");
        certificate.PhoneNumber = Input.OptionalPhone(request.PhoneNumber, "Telefon numarası");
    }

    private void DeleteImageFile(string? fileName)
    {
        if (fileName is null)
            return;

        var path = Path.Combine(ImageDirectory, fileName);
        if (File.Exists(path))
            File.Delete(path);
    }

    private async Task<Certificate> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Sertifika bulunamadı.");
}
