using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Context;
using Ygt.WebAPI.Extensions;
using Ygt.WebAPI.Middlewares;
using Ygt.WebAPI.Options;
using Ygt.WebAPI.Providers;
using Ygt.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("ConnectionStrings:SqlServer ayarlanmamış.");

// EnableRetryOnFailure: Docker'da SQL Server API'den geç açılırsa bağlantı birkaç kez yeniden denenir.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.Configure<AdminSeedOptions>(builder.Configuration.GetSection("Seed:Admin"));
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SiteService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<FormService>();
builder.Services.AddScoped<CertificateService>();
builder.Services.AddScoped<SiteContentService>();
builder.Services.AddScoped<AboutCardService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<TeamMemberService>();
builder.Services.AddScoped<TechnologyService>();
builder.Services.AddScoped<SiteLinkService>();

builder.Services.AddScoped<JwtProvider>();
builder.Services.AddSingleton<ExcelProvider>();
builder.Services.AddSingleton<ImageStorageProvider>();

builder.Services.AddYgtControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddYgtRateLimiting(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// Geliştirmede Angular dev sunucusu farklı porttan istek atar.
// Docker'da ön yüz ve API aynı adresten sunulacağı için orada CORS gerekmez.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("Content-Disposition")));

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(options => options.SecretKey?.Length >= 64, "Jwt:SecretKey en az 64 karakter olmalı.")
    .ValidateOnStart();
builder.Services.ConfigureOptions<JwtSetupOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

// Docker'da API, ön yüzü sunan nginx'in arkasında çalışacak. İstek sınırının gerçek istemci IP'sine
// uygulanması için IP, X-Forwarded-For başlığından okunur. API portu dışarı açılmadığı sürece güvenli.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseYgtStatusCodeMessages();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
