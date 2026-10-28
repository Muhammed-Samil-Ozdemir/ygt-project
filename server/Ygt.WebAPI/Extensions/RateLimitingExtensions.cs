using System.Threading.RateLimiting;
using Ygt.WebAPI.Dtos;

namespace Ygt.WebAPI.Extensions;

public static class RateLimitPolicies
{
    public const string Applications = "applications";
    public const string Login = "login";
}

public static class RateLimitingExtensions
{
    /// <summary>
    /// IP başına dakikalık istek sınırı. Eski sistemdeki form oturum token'ının yerine spam'e karşı koruma sağlar.
    /// Sınıfta duyuru yapıldığında aynı kampüs ağından (aynı IP'den) çok sayıda öğrenci birlikte
    /// başvurabildiği için başvuru sınırı geniş tutuldu.
    /// </summary>
    public static IServiceCollection AddYgtRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationsPerMinute = configuration.GetValue("RateLimiting:ApplicationsPerMinute", 60);
        var loginPerMinute = configuration.GetValue("RateLimiting:LoginPerMinute", 5);

        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) => new ValueTask(
                context.HttpContext.Response.WriteAsJsonAsync(
                    new MessageDto("Çok fazla istek gönderdiniz. Lütfen biraz sonra tekrar deneyin."),
                    cancellationToken));

            options.AddPolicy(RateLimitPolicies.Applications, context => PerIpPerMinute(context, applicationsPerMinute));
            options.AddPolicy(RateLimitPolicies.Login, context => PerIpPerMinute(context, loginPerMinute));
        });
    }

    private static RateLimitPartition<string> PerIpPerMinute(HttpContext context, int permitLimit) =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
}
