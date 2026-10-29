using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class CertificateService(
    AppDbContext dbContext,
    ImageStorageProvider imageStorage,
    ExcelProvider excelProvider)
{
    // Toplu ekleme şablonunun sütunları.
    private const string CourseNameHeader = "Eğitim adı";
    private const string FirstNameHeader = "Ad";
    private const string LastNameHeader = "Soyad";
    private const string EmailHeader = "E-posta";
    private const string PhoneHeader = "Telefon";

    private static readonly string[] ImportHeaders =
        [CourseNameHeader, FirstNameHeader, LastNameHeader, EmailHeader, PhoneHeader];

    private const int MaxImportRows = 1000;

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

    /// <summary>Toplu ekleme için boş Excel şablonu: sadece başlık satırı.</summary>
    public FileDto GetImportTemplate() =>
        excelProvider.CreateFile(
            "sertifika-sablonu.xlsx",
            "Sertifikalar",
            Array.Empty<string>(),
            ImportHeaders.Select(header => (header, (Func<string, string?>)(_ => null))).ToArray());

    /// <summary>
    /// Excel'deki her satır için bir sertifika oluşturur. Bir satır bile hatalıysa hiçbiri eklenmez ve hatalı
    /// satırlar numarasıyla bildirilir; dosya düzeltilip tekrar yüklenir.
    /// </summary>
    public async Task<MessageDto> ImportAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var rows = excelProvider.ReadRows(file, ImportHeaders, MaxImportRows);
        if (rows.Count == 0)
            throw Errors.BadRequest("Excel'de sertifika satırı bulunamadı.");

        var certificates = new List<Certificate>();
        var errors = new List<string>();

        foreach (var row in rows)
        {
            try
            {
                var certificate = new Certificate { Code = Guid.NewGuid().ToString() };
                Fill(certificate, new CertificateCommandDto(
                    row[CourseNameHeader], row[FirstNameHeader], row[LastNameHeader], row[EmailHeader], row[PhoneHeader]));
                certificates.Add(certificate);
            }
            catch (BadHttpRequestException exception)
            {
                errors.Add($"{row.Number}. satır: {exception.Message}");
            }
        }

        if (errors.Count == 1)
            throw Errors.BadRequest($"{errors[0]} Hiçbir sertifika eklenmedi.");

        if (errors.Count > 1)
        {
            var shown = string.Join(" ", errors.Take(5));
            var more = errors.Count > 5 ? $" (ve {errors.Count - 5} satır daha)" : "";
            throw Errors.BadRequest($"{errors.Count} satırda hata var, hiçbir sertifika eklenmedi. {shown}{more}");
        }

        dbContext.Certificates.AddRange(certificates);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MessageDto($"{certificates.Count} sertifika eklendi.");
    }

    /// <summary>
    /// Bütün sertifikaları doğrulama linkleriyle Excel'e aktarır (kişilere link dağıtmak için).
    /// siteUrl: sitenin adresi, ör. "https://yazilimgelistirme.ktun.edu.tr"; panel kendi adresini gönderir.
    /// </summary>
    public async Task<FileDto> ExportAsync(string? siteUrl, CancellationToken cancellationToken)
    {
        var baseUrl = Input.RequiredUrl(siteUrl, "Site adresi").TrimEnd('/');

        var certificates = await dbContext.Certificates.AsNoTracking()
            .OrderBy(c => c.CourseName)
            .ThenBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);

        return excelProvider.CreateFile("sertifikalar.xlsx", "Sertifikalar", certificates,
            (FirstNameHeader, c => c.FirstName),
            (LastNameHeader, c => c.LastName),
            (CourseNameHeader, c => c.CourseName),
            (EmailHeader, c => c.Email),
            (PhoneHeader, c => c.PhoneNumber),
            ("Doğrulama linki", c => $"{baseUrl}/certificates/{Uri.EscapeDataString(c.Code)}"),
            ("Oluşturulma", c => TurkeyTime.Format(c.CreatedAt)));
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
