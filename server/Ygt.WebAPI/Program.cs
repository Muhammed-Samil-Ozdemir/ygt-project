using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("ConnectionStrings:SqlServer ayarlanmamış.");

// EnableRetryOnFailure: Docker'da SQL Server API'den geç açılırsa bağlantı birkaç kez yeniden denenir.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.Configure<AdminSeedOptions>(builder.Configuration.GetSection("Seed:Admin"));

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.Run();
