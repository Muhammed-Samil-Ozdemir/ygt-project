namespace Ygt.WebAPI.Utilities;

/// <summary>
/// Veritabanındaki UTC zamanları Türkiye saatiyle yazar (UTC+3, yaz saati uygulaması yok).
/// Excel gibi insanların okuyacağı çıktılarda kullanılır.
/// </summary>
public static class TurkeyTime
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(3);

    public static string Format(DateTime utc) => utc.Add(Offset).ToString("dd.MM.yyyy HH:mm");
}
