using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Abstractions;
using Ygt.WebAPI.Models;

namespace Ygt.WebAPI.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseApplication> CourseApplications => Set<CourseApplication>();
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<FormApplication> FormApplications => Set<FormApplication>();
    public DbSet<HackathonApplication> HackathonApplications => Set<HackathonApplication>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<VisitorCounter> VisitorCounters => Set<VisitorCounter>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQL Server tarihleri saat dilimi bilgisi olmadan döndürür. Hepsini UTC olarak işaretliyoruz,
        // böylece JSON'a "Z" ekiyle yazılır ve ön yüz doğru yerel saati gösterir.
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
