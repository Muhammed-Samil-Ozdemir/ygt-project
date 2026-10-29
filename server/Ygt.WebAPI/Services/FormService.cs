using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class FormService(AppDbContext dbContext, ExcelProvider excelProvider)
{
    private const string ClosedMessage = "Başvurular sona ermiştir. İlginiz için teşekkür ederiz.";

    public async Task<FormDto> GetActiveAsync(CancellationToken cancellationToken) =>
        await dbContext.Forms
            .Where(f => f.IsActive)
            .Select(f => new FormDto(f.Id, f.Title, f.Content, f.Type))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw Errors.NotFound(ClosedMessage);

    public async Task<List<FormListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Forms
            .OrderByDescending(f => f.IsActive)
            .ThenByDescending(f => f.CreatedAt)
            .Select(f => new FormListItemDto(
                f.Id,
                f.Title,
                f.Content,
                f.Type,
                f.IsActive,
                f.Type == FormType.Hackathon ? f.HackathonApplications.Count : f.Applications.Count,
                f.CreatedAt,
                f.UpdatedAt))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Yeni form aktif açılır, önceki aktif form kapanır ve ana sayfa butonu etkinlik başvurusuna çevrilir
    /// (eski sistemdeki davranış).
    /// </summary>
    public async Task<FormDto> CreateAsync(FormCommandDto request, CancellationToken cancellationToken)
    {
        var form = new Form
        {
            Title = Input.RequiredText(request.Title, "Başlık", 3, 255),
            Content = Input.OptionalText(request.Content, "Açıklama", 4000),
            Type = request.Type ?? FormType.Standard
        };

        await ActivateExclusivelyAsync(async () =>
        {
            dbContext.Forms.Add(form);
            await SetHomeButtonAsync(HomeButton.Form, cancellationToken);
        }, cancellationToken);

        return new FormDto(form.Id, form.Title, form.Content, form.Type);
    }

    public async Task UpdateAsync(Guid id, FormCommandDto request, CancellationToken cancellationToken)
    {
        var form = await FindAsync(id, cancellationToken);

        form.Title = Input.RequiredText(request.Title, "Başlık", 3, 255);
        form.Content = Input.OptionalText(request.Content, "Açıklama", 4000);

        if (request.Type is { } type && type != form.Type)
        {
            var hasApplications =
                await dbContext.FormApplications.AnyAsync(a => a.FormId == id, cancellationToken)
                || await dbContext.HackathonApplications.AnyAsync(a => a.FormId == id, cancellationToken);

            if (hasApplications)
                throw Errors.Conflict("Başvurusu olan formun türü değiştirilemez.");

            form.Type = type;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Form kapanınca ana sayfa butonu eğitim başvurusuna döner (eski sistemdeki davranış).</summary>
    public async Task CloseAsync(Guid id, CancellationToken cancellationToken)
    {
        var form = await FindAsync(id, cancellationToken);
        if (!form.IsActive)
            return;

        form.IsActive = false;
        await SetHomeButtonAsync(HomeButton.Course, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Formu yeniden açar. Aynı anda tek aktif form olabildiği için o an açık olan form kapanır.</summary>
    public async Task ReopenAsync(Guid id, CancellationToken cancellationToken)
    {
        var form = await FindAsync(id, cancellationToken);
        if (form.IsActive)
            return;

        await ActivateExclusivelyAsync(async () =>
        {
            form.IsActive = true;
            await SetHomeButtonAsync(HomeButton.Form, cancellationToken);
        }, cancellationToken);
    }

    public async Task ApplyAsync(Guid formId, FormApplicationCommandDto request, CancellationToken cancellationToken)
    {
        var application = new FormApplication
        {
            FormId = formId,
            FirstName = Input.RequiredText(request.FirstName, "Ad", 2, 50),
            LastName = Input.RequiredText(request.LastName, "Soyad", 2, 50),
            Email = Input.OptionalEmail(request.Email, "E-posta"),
            PhoneNumber = Input.RequiredPhone(request.PhoneNumber, "Telefon numarası")
        };

        await EnsureOpenAsync(formId, FormType.Standard, cancellationToken);

        var alreadyApplied = await dbContext.FormApplications.AnyAsync(a =>
            a.FormId == formId
            && (a.PhoneNumber == application.PhoneNumber
                || (application.Email != null && a.Email == application.Email)),
            cancellationToken);

        if (alreadyApplied)
            throw Errors.Conflict("Bu forma daha önce başvurdunuz.");

        dbContext.FormApplications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyHackathonAsync(Guid formId, HackathonApplicationCommandDto request, CancellationToken cancellationToken)
    {
        var teamName = Input.RequiredText(request.TeamName, "Takım adı", 3, 100);

        List<HackathonMember> members =
        [
            ReadMember(request.Member1, 1)!,
            ReadMember(request.Member2, 2)!
        ];

        if (ReadMember(request.Member3, 3, isOptional: true) is { } thirdMember)
            members.Add(thirdMember);

        if (members.DistinctBy(m => m.Phone).Count() != members.Count)
            throw Errors.BadRequest("Birden fazla üye için aynı telefon numarasını girdiniz.");

        if (members.DistinctBy(m => m.Email).Count() != members.Count)
            throw Errors.BadRequest("Birden fazla üye için aynı e-posta adresini girdiniz.");

        await EnsureOpenAsync(formId, FormType.Hackathon, cancellationToken);

        if (await dbContext.HackathonApplications.AnyAsync(a => a.FormId == formId && a.TeamName == teamName, cancellationToken))
            throw Errors.Conflict("Bu takım adı daha önce alınmış.");

        // Üyelerden biri bu forma başka bir takımla başvurmuş mu?
        var phones = members.Select(m => m.Phone).ToList();
        var emails = members.Select(m => m.Email).ToList();

        var existingTeams = await dbContext.HackathonApplications.AsNoTracking()
            .Where(a => a.FormId == formId
                && (phones.Contains(a.Member1Phone) || phones.Contains(a.Member2Phone) || phones.Contains(a.Member3Phone!)
                    || emails.Contains(a.Member1Email) || emails.Contains(a.Member2Email) || emails.Contains(a.Member3Email!)))
            .ToListAsync(cancellationToken);

        var usedPhones = existingTeams.SelectMany(a => new[] { a.Member1Phone, a.Member2Phone, a.Member3Phone }).ToHashSet();
        var usedEmails = existingTeams.SelectMany(a => new[] { a.Member1Email, a.Member2Email, a.Member3Email }).ToHashSet();

        var registeredMember = members.FirstOrDefault(m => usedPhones.Contains(m.Phone) || usedEmails.Contains(m.Email));
        if (registeredMember is not null)
            throw Errors.Conflict($"\"{registeredMember.Name}\" adlı üyenin telefon numarası veya e-postasıyla daha önce başvuru yapılmış.");

        dbContext.HackathonApplications.Add(new HackathonApplication
        {
            FormId = formId,
            TeamName = teamName,
            Member1Name = members[0].Name,
            Member1Phone = members[0].Phone,
            Member1Email = members[0].Email,
            Member2Name = members[1].Name,
            Member2Phone = members[1].Phone,
            Member2Email = members[1].Email,
            Member3Name = members.ElementAtOrDefault(2)?.Name,
            Member3Phone = members.ElementAtOrDefault(2)?.Phone,
            Member3Email = members.ElementAtOrDefault(2)?.Email
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<FormApplicationDto>> GetApplicationsAsync(Guid formId, CancellationToken cancellationToken)
    {
        await FindAsync(formId, cancellationToken);

        return await dbContext.FormApplications
            .Where(a => a.FormId == formId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new FormApplicationDto(a.Id, a.FirstName, a.LastName, a.Email, a.PhoneNumber, a.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<HackathonApplicationDto>> GetHackathonApplicationsAsync(Guid formId, CancellationToken cancellationToken)
    {
        await FindAsync(formId, cancellationToken);

        var applications = await dbContext.HackathonApplications.AsNoTracking()
            .Where(a => a.FormId == formId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return applications.Select(a => new HackathonApplicationDto(
                a.Id,
                a.TeamName,
                new HackathonMemberDto(a.Member1Name, a.Member1Phone, a.Member1Email),
                new HackathonMemberDto(a.Member2Name, a.Member2Phone, a.Member2Email),
                a.Member3Name is null ? null : new HackathonMemberDto(a.Member3Name, a.Member3Phone, a.Member3Email),
                a.CreatedAt))
            .ToList();
    }

    public async Task DeleteApplicationAsync(Guid formId, Guid applicationId, CancellationToken cancellationToken)
    {
        var deleted = await dbContext.FormApplications
            .Where(a => a.FormId == formId && a.Id == applicationId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
            throw Errors.NotFound("Başvuru bulunamadı.");
    }

    public async Task DeleteHackathonApplicationAsync(Guid formId, Guid applicationId, CancellationToken cancellationToken)
    {
        var deleted = await dbContext.HackathonApplications
            .Where(a => a.FormId == formId && a.Id == applicationId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
            throw Errors.NotFound("Takım başvurusu bulunamadı.");
    }

    /// <summary>
    /// Formun bütün başvurularını (bireysel ya da takım) kalıcı olarak siler (KVKK). Formun kendisi kalır.
    /// Silinen başvuru sayısını döndürür.
    /// </summary>
    public async Task<int> DeleteApplicationsAsync(Guid formId, CancellationToken cancellationToken)
    {
        await FindAsync(formId, cancellationToken);

        var applications = await dbContext.FormApplications
            .Where(a => a.FormId == formId)
            .ExecuteDeleteAsync(cancellationToken);

        var teams = await dbContext.HackathonApplications
            .Where(a => a.FormId == formId)
            .ExecuteDeleteAsync(cancellationToken);

        return applications + teams;
    }

    /// <summary>Formun türüne göre bireysel ya da takım başvurularını Excel'e aktarır.</summary>
    public async Task<FileDto> ExportApplicationsAsync(Guid formId, CancellationToken cancellationToken)
    {
        var form = await FindAsync(formId, cancellationToken);

        if (form.Type == FormType.Hackathon)
        {
            var teams = await dbContext.HackathonApplications.AsNoTracking()
                .Where(a => a.FormId == formId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync(cancellationToken);

            return excelProvider.Create(form.Title, teams,
                ("Takım Adı", a => a.TeamName),
                ("1. Üye", a => a.Member1Name),
                ("1. Üye Telefon", a => a.Member1Phone),
                ("1. Üye E-posta", a => a.Member1Email),
                ("2. Üye", a => a.Member2Name),
                ("2. Üye Telefon", a => a.Member2Phone),
                ("2. Üye E-posta", a => a.Member2Email),
                ("3. Üye", a => a.Member3Name),
                ("3. Üye Telefon", a => a.Member3Phone),
                ("3. Üye E-posta", a => a.Member3Email),
                ("Başvuru Tarihi", a => TurkeyTime.Format(a.CreatedAt)));
        }

        var applications = await dbContext.FormApplications.AsNoTracking()
            .Where(a => a.FormId == formId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return excelProvider.Create(form.Title, applications,
            ("Ad", a => a.FirstName),
            ("Soyad", a => a.LastName),
            ("Telefon", a => a.PhoneNumber),
            ("E-posta", a => a.Email),
            ("Başvuru Tarihi", a => TurkeyTime.Format(a.CreatedAt)));
    }

    /// <summary>
    /// Bir takım üyesinin alanlarını doğrular. İsteğe bağlı üyenin hiçbir alanı doldurulmamışsa null döner;
    /// bir alanı bile doldurulmuşsa hepsi zorunludur.
    /// </summary>
    private static HackathonMember? ReadMember(HackathonMemberDto? member, int number, bool isOptional = false)
    {
        if (isOptional
            && string.IsNullOrWhiteSpace(member?.Name)
            && string.IsNullOrWhiteSpace(member?.Phone)
            && string.IsNullOrWhiteSpace(member?.Email))
            return null;

        var name = Input.RequiredText(member?.Name, $"{number}. üyenin adı", 3, 100);
        var phone = Input.RequiredPhone(member?.Phone, $"{number}. üyenin telefon numarası");
        var email = Input.RequiredEmail(member?.Email, $"{number}. üyenin e-postası");

        if (!email.Contains(".edu"))
            throw Errors.BadRequest($"{number}. üyenin e-postası '.edu' uzantılı okul e-postası olmalıdır.");

        return new HackathonMember(name, phone, email);
    }

    private async Task EnsureOpenAsync(Guid formId, FormType expectedType, CancellationToken cancellationToken)
    {
        var form = await dbContext.Forms.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == formId, cancellationToken);

        if (form is null || !form.IsActive)
            throw Errors.NotFound(ClosedMessage);

        if (form.Type != expectedType)
            throw Errors.BadRequest(expectedType == FormType.Hackathon
                ? "Bu form takım başvurusu almıyor."
                : "Bu form takım başvurusu alıyor.");
    }

    /// <summary>
    /// Açık formları kapatır, ardından verilen değişiklikleri kaydeder; ikisi tek transaction'da.
    /// "Tek aktif form" index'i yüzünden kapatma, yeni form açılmadan önce ayrı bir komutla veritabanına gitmeli.
    /// Aynı SaveChanges'e konduğunda EF güncellemeleri ters sırada gönderebiliyor ve index ihlali oluşuyor.
    /// </summary>
    private async Task ActivateExclusivelyAsync(Func<Task> applyChanges, CancellationToken cancellationToken)
    {
        // Bağlantı hatasında yeniden deneme açık olduğu için transaction, execution strategy içinde açılmalı.
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            await dbContext.Forms
                .Where(f => f.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(f => f.IsActive, false)
                    .SetProperty(f => f.UpdatedAt, DateTime.UtcNow), cancellationToken);

            await applyChanges();
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SetHomeButtonAsync(HomeButton homeButton, CancellationToken cancellationToken)
    {
        var settings = await dbContext.SiteSettings.FirstAsync(cancellationToken);
        settings.HomeButton = homeButton;
    }

    private async Task<Form> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Forms.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Form bulunamadı.");

    private sealed record HackathonMember(string Name, string Phone, string Email);
}
