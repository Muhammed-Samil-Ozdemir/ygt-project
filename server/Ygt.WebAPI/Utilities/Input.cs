using System.Text.RegularExpressions;

namespace Ygt.WebAPI.Utilities;

/// <summary>
/// İstekten gelen alanları temizler ve doğrular. Geçersiz değerde Türkçe mesajlı 400 hatası fırlatır.
/// Boş ya da sadece boşluktan oluşan değerler girilmemiş sayılır.
/// </summary>
public static partial class Input
{
    public static string RequiredText(string? value, string label, int minLength, int maxLength)
    {
        var text = Clean(value) ?? throw Errors.BadRequest($"{label} zorunludur.");

        if (text.Length < minLength || text.Length > maxLength)
            throw Errors.BadRequest($"{label} {minLength}-{maxLength} karakter arasında olmalıdır.");

        return text;
    }

    public static string? OptionalText(string? value, string label, int maxLength)
    {
        var text = Clean(value);

        if (text is not null && text.Length > maxLength)
            throw Errors.BadRequest($"{label} en fazla {maxLength} karakter olabilir.");

        return text;
    }

    public static string RequiredEmail(string? value, string label) =>
        OptionalEmail(value, label) ?? throw Errors.BadRequest($"{label} zorunludur.");

    /// <summary>E-posta küçük harfe çevrilir, böylece tekrar kontrolleri harf büyüklüğünden etkilenmez.</summary>
    public static string? OptionalEmail(string? value, string label)
    {
        var email = Clean(value)?.ToLowerInvariant();
        if (email is null)
            return null;

        if (email.Length > 256 || !EmailPattern().IsMatch(email))
            throw Errors.BadRequest($"{label} geçerli değil.");

        return email;
    }

    public static string RequiredPhone(string? value, string label) =>
        OptionalPhone(value, label) ?? throw Errors.BadRequest($"{label} zorunludur.");

    /// <summary>
    /// Telefonu başında 0 veya +90 olmadan 10 haneye çevirir: "0 (555) 123-45-67" -> "5551234567".
    /// Aynı numaranın farklı yazımları tekrar kontrolünde aynı sayılsın diye hep bu biçimde saklanır.
    /// </summary>
    public static string? OptionalPhone(string? value, string label)
    {
        var text = Clean(value);
        if (text is null)
            return null;

        if (!PhonePattern().IsMatch(text))
            throw Invalid();

        var digits = new string(text.Where(char.IsAsciiDigit).ToArray());

        if (digits.Length == 12 && digits.StartsWith("90"))
            digits = digits[2..];
        else if (digits.Length == 11 && digits.StartsWith('0'))
            digits = digits[1..];

        if (digits.Length != 10 || digits[0] == '0')
            throw Invalid();

        return digits;

        BadHttpRequestException Invalid() => Errors.BadRequest($"{label} geçerli değil. Örnek: 0555 123 45 67");
    }

    public static string RequiredUrl(string? value, string label) =>
        OptionalUrl(value, label) ?? throw Errors.BadRequest($"{label} zorunludur.");

    /// <summary>
    /// Sadece http(s) ve mailto bağlantıları kabul edilir. "javascript:" gibi adresler sitede
    /// tıklanınca kod çalıştırabileceği için reddedilir.
    /// </summary>
    public static string? OptionalUrl(string? value, string label)
    {
        var url = Clean(value);
        if (url is null)
            return null;

        if (url.Length > 500
            || !Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https" or "mailto"))
            throw Errors.BadRequest($"{label} geçerli değil. Örnek: https://www.instagram.com/ktunygt/");

        return url;
    }

    /// <summary>TC kimlik numarasını biçimine ve kontrol hanelerine göre doğrular.</summary>
    public static string? OptionalTcNumber(string? value)
    {
        var tcNumber = Clean(value);
        if (tcNumber is null)
            return null;

        if (!IsValidTcNumber(tcNumber))
            throw Errors.BadRequest("TC kimlik numarası geçerli değil.");

        return tcNumber;
    }

    private static bool IsValidTcNumber(string tcNumber)
    {
        if (tcNumber.Length != 11 || !tcNumber.All(char.IsAsciiDigit) || tcNumber[0] == '0')
            return false;

        var d = tcNumber.Select(c => c - '0').ToArray();
        var oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
        var evenSum = d[1] + d[3] + d[5] + d[7];
        var tenthDigit = ((oddSum * 7 - evenSum) % 10 + 10) % 10;
        var eleventhDigit = d.Take(10).Sum() % 10;

        return d[9] == tenthDigit && d[10] == eleventhDigit;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();

    [GeneratedRegex(@"^\+?[0-9\s\-()]+$")]
    private static partial Regex PhonePattern();
}
