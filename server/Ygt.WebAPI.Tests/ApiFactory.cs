using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Ygt.WebAPI.Tests;

/// <summary>
/// API'yi testlerde gerçek bir SQL Server'la çalıştırır. SQL Server, Testcontainers ile geçici bir Docker konteynerinde
/// açılır ve testler bitince silinir; bu yüzden testler için Docker çalışıyor olmalı.
/// Bütün entegrasyon testleri aynı API'yi ve veritabanını paylaşır, sırayla çalışır (ApiCollection).
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string AdminEmail = "admin@test.local";
    public const string AdminPassword = "Test_Admin_2026";

    private readonly MsSqlContainer _database = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").Build();
    private readonly string _storagePath = Path.Combine(Path.GetTempPath(), $"ygt-tests-{Guid.NewGuid():N}");

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        var connectionString = new SqlConnectionStringBuilder(_database.GetConnectionString())
        {
            InitialCatalog = "YgtTests"
        }.ConnectionString;

        // Program.cs ayarları uygulama kurulurken okuduğu için ortam değişkeniyle verilir.
        Environment.SetEnvironmentVariable("ConnectionStrings__SqlServer", connectionString);
        Environment.SetEnvironmentVariable("Jwt__SecretKey", new string('t', 64) + "-yalnizca-testler-icin");
        Environment.SetEnvironmentVariable("Seed__Admin__Email", AdminEmail);
        Environment.SetEnvironmentVariable("Seed__Admin__Password", AdminPassword);
        Environment.SetEnvironmentVariable("RateLimiting__ApplicationsPerMinute", "100000");
        Environment.SetEnvironmentVariable("RateLimiting__LoginPerMinute", "100000");
        Environment.SetEnvironmentVariable("Storage__RootPath", _storagePath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        // Development ayarları (yerel bağlantı cümlesi) yüklenmesin.
        builder.UseEnvironment("Testing");

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();

        if (Directory.Exists(_storagePath))
            Directory.Delete(_storagePath, recursive: true);
    }
}

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "api";
}
