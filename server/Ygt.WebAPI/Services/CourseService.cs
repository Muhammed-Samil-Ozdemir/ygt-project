using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Enums;
using Ygt.WebAPI.Models;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Services;

public sealed class CourseService(AppDbContext dbContext, ExcelProvider excelProvider)
{
    public async Task<List<CourseDto>> GetActiveAsync(CancellationToken cancellationToken) =>
        await dbContext.Courses
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto(c.Id, c.Title, c.Content))
            .ToListAsync(cancellationToken);

    public async Task<List<CourseListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Courses
            .OrderByDescending(c => c.IsActive)
            .ThenByDescending(c => c.CreatedAt)
            .Select(c => new CourseListItemDto(
                c.Id,
                c.Title,
                c.Content,
                c.IsActive,
                c.Applications.Count,
                c.CreatedAt,
                c.UpdatedAt))
            .ToListAsync(cancellationToken);

    /// <summary>Yeni kurs aktif açılır ve ana sayfa butonu eğitim başvurusuna çevrilir (eski sistemdeki davranış).</summary>
    public async Task<CourseDto> CreateAsync(CourseCommandDto request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Title = Input.RequiredText(request.Title, "Başlık", 3, 255),
            Content = Input.OptionalText(request.Content, "Açıklama", 4000)
        };

        dbContext.Courses.Add(course);

        var settings = await dbContext.SiteSettings.FirstAsync(cancellationToken);
        settings.HomeButton = HomeButton.Course;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CourseDto(course.Id, course.Title, course.Content);
    }

    public async Task UpdateAsync(Guid id, CourseCommandDto request, CancellationToken cancellationToken)
    {
        var course = await FindAsync(id, cancellationToken);

        course.Title = Input.RequiredText(request.Title, "Başlık", 3, 255);
        course.Content = Input.OptionalText(request.Content, "Açıklama", 4000);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var course = await FindAsync(id, cancellationToken);
        course.IsActive = isActive;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyAsync(Guid courseId, CourseApplicationCommandDto request, CancellationToken cancellationToken)
    {
        var application = new CourseApplication
        {
            CourseId = courseId,
            FirstName = Input.RequiredText(request.FirstName, "Ad", 2, 50),
            LastName = Input.RequiredText(request.LastName, "Soyad", 2, 50),
            TcNumber = Input.OptionalTcNumber(request.TcNumber),
            Email = Input.RequiredEmail(request.Email, "E-posta"),
            PhoneNumber = Input.OptionalPhone(request.PhoneNumber, "Telefon numarası"),
            BirthDate = request.BirthDate
        };

        if (application.BirthDate is { } birthDate
            && (birthDate >= DateOnly.FromDateTime(DateTime.UtcNow) || birthDate.Year < 1900))
            throw Errors.BadRequest("Doğum tarihi geçerli değil.");

        var course = await dbContext.Courses.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken)
            ?? throw Errors.NotFound("Böyle bir eğitim bulunamadı.");

        if (!course.IsActive)
            throw Errors.NotFound("Bu eğitimin başvuruları sona ermiştir.");

        var alreadyApplied = await dbContext.CourseApplications.AnyAsync(a =>
            a.CourseId == courseId
            && (a.Email == application.Email
                || (application.PhoneNumber != null && a.PhoneNumber == application.PhoneNumber)
                || (application.TcNumber != null && a.TcNumber == application.TcNumber)),
            cancellationToken);

        if (alreadyApplied)
            throw Errors.Conflict("Bu eğitime daha önce başvurdunuz.");

        dbContext.CourseApplications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CourseApplicationDto>> GetApplicationsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        await FindAsync(courseId, cancellationToken);

        return await dbContext.CourseApplications
            .Where(a => a.CourseId == courseId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new CourseApplicationDto(
                a.Id,
                a.FirstName,
                a.LastName,
                a.TcNumber,
                a.Email,
                a.PhoneNumber,
                a.BirthDate,
                a.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<FileDto> ExportApplicationsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await FindAsync(courseId, cancellationToken);

        var applications = await dbContext.CourseApplications.AsNoTracking()
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return excelProvider.Create(course.Title, applications,
            ("Ad", a => a.FirstName),
            ("Soyad", a => a.LastName),
            ("TC Kimlik No", a => a.TcNumber),
            ("E-posta", a => a.Email),
            ("Telefon", a => a.PhoneNumber),
            ("Doğum Tarihi", a => a.BirthDate?.ToString("dd.MM.yyyy")),
            ("Başvuru Tarihi", a => TurkeyTime.Format(a.CreatedAt)));
    }

    private async Task<Course> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
        ?? throw Errors.NotFound("Kurs bulunamadı.");
}
