using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class CertificateService(AppDbContext dbContext, ImageStorageProvider imageStorage)
{
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

        return await imageStorage.ReadAsync(
            ImageStorageProvider.Certificates, fileName, "Sertifika görseli bulunamadı.", cancellationToken);
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

        imageStorage.Delete(ImageStorageProvider.Certificates, certificate.ImageFileName);
    }

    /// <summary>Görseli sertifika ID'siyle adlandırıp kaydeder. Önceki görsel varsa yerine geçer.</summary>
    public async Task UploadImageAsync(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        var certificate = await FindAsync(id, cancellationToken);

        var fileName = await imageStorage.SaveAsync(
            ImageStorageProvider.Certificates, certificate.Id.ToString(), file, cancellationToken);

        if (certificate.ImageFileName != fileName)
            imageStorage.Delete(ImageStorageProvider.Certificates, certificate.ImageFileName);

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

    private async Task<Certificate> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Sertifika bulunamadı.");
}
