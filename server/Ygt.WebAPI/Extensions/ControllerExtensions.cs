using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ygt.WebAPI.Dtos;

namespace Ygt.WebAPI.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddYgtControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
                // Alan doğrulaması servislerde, Türkçe mesajlarla yapılıyor.
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .AddJsonOptions(options =>
                // Enum'lar JSON'da metin olarak gider: "hackathon", "course"...
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)))
            .ConfigureApiBehaviorOptions(options =>
            {
                // Buraya sadece okunamayan JSON düşer (yanlış tip, bozuk gövde). Ayrıntı İngilizce olduğu için genel mesaj dönülür.
                options.InvalidModelStateResponseFactory = _ => new BadRequestObjectResult(
                    new MessageDto("Gönderilen veri okunamadı. Alanları kontrol edip tekrar deneyin."));

                // 415 gibi hatalarda ProblemDetails yerine boş gövde dönsün; mesajı UseYgtStatusCodeMessages ekler.
                options.SuppressMapClientErrors = true;
            });

        return services;
    }

    /// <summary>
    /// Gövdesi boş hata yanıtlarına (bilinmeyen adres, desteklenmeyen istek türü...) { message } ekler.
    /// Böylece ön yüz bütün hataları aynı biçimde okur.
    /// </summary>
    public static IApplicationBuilder UseYgtStatusCodeMessages(this IApplicationBuilder app) =>
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            var message = response.StatusCode switch
            {
                StatusCodes.Status404NotFound => "İstenen adres bulunamadı.",
                StatusCodes.Status405MethodNotAllowed => "Bu adres bu istek türünü desteklemiyor.",
                StatusCodes.Status415UnsupportedMediaType => "İstek gövdesi JSON olarak gönderilmelidir.",
                _ => "İstek işlenemedi."
            };

            await response.WriteAsJsonAsync(new MessageDto(message));
        });
}
