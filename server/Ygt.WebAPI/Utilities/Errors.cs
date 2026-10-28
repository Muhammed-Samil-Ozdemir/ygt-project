namespace Ygt.WebAPI.Utilities;

/// <summary>
/// Servislerin fırlattığı hatalar. GlobalExceptionHandler bunları verilen durum koduyla
/// ve { message } gövdesiyle istemciye döndürür.
/// </summary>
public static class Errors
{
    public static BadHttpRequestException BadRequest(string message) => new(message, StatusCodes.Status400BadRequest);
    public static BadHttpRequestException Unauthorized(string message) => new(message, StatusCodes.Status401Unauthorized);
    public static BadHttpRequestException NotFound(string message) => new(message, StatusCodes.Status404NotFound);
    public static BadHttpRequestException Conflict(string message) => new(message, StatusCodes.Status409Conflict);
}
