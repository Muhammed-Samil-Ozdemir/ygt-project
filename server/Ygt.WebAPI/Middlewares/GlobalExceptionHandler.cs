using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ygt.WebAPI.Dtos;

namespace Ygt.WebAPI.Middlewares;

/// <summary>
/// Bütün hataları { message } gövdesiyle döndürür; ön yüz mesajı doğrudan kullanıcıya gösterir.
/// Beklenmeyen hatalarda iç ayrıntı gösterilmez, sadece loglanır.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            BadHttpRequestException badRequest => (badRequest.StatusCode, badRequest.Message),

            // Unique index ihlali: aynı anda gelen iki istek, servisteki tekrar kontrolünü birlikte geçmiş olabilir.
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                (StatusCodes.Status409Conflict, "Bu kayıt zaten mevcut."),

            _ => (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "İstek işlenirken beklenmeyen bir hata oluştu.");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new MessageDto(message), cancellationToken);
        return true;
    }
}
