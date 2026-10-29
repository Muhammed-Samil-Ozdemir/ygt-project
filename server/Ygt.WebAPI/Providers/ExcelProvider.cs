using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Ygt.WebAPI.Dtos;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Providers;

/// <summary>Excel (.xlsx) dosyası yazar (başvuru ve sertifika listeleri) ve okur (sertifika toplu ekleme).</summary>
public sealed partial class ExcelProvider
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    /// <summary>Başvuru listesi.</summary>
    /// <param name="title">Dosya adında kullanılır, ör. "Hackathon 2026" -> "Hackathon 2026 - başvurular.xlsx".</param>
    /// <param name="columns">Sütun başlığı ve her satır için o sütunun değeri.</param>
    public FileDto Create<T>(string title, IReadOnlyList<T> rows, params (string Header, Func<T, string?> Value)[] columns) =>
        CreateFile($"{InvalidFileNameChars().Replace(title, "")} - başvurular.xlsx", "Başvurular", rows, columns);

    public FileDto CreateFile<T>(
        string fileName,
        string sheetName,
        IReadOnlyList<T> rows,
        params (string Header, Func<T, string?> Value)[] columns)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(sheetName);

        for (var column = 0; column < columns.Length; column++)
        {
            sheet.Cell(1, column + 1).Value = columns[column].Header;
        }

        for (var row = 0; row < rows.Count; row++)
        {
            for (var column = 0; column < columns.Length; column++)
            {
                sheet.Cell(row + 2, column + 1).Value = columns[column].Value(rows[row]) ?? "";
            }
        }

        sheet.Row(1).Style.Font.Bold = true;
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new FileDto(stream.ToArray(), ContentType, fileName);
    }

    /// <summary>
    /// İlk sayfadaki tabloyu okur. İlk dolu satır başlık satırıdır; başlıklar büyük/küçük harfe bakılmadan
    /// <paramref name="headers"/> ile eşleştirilir, sütun sırası önemli değildir. Tamamen boş satırlar atlanır.
    /// Her satırın değerleri başlık adıyla (headers'taki yazımıyla) gelir; hücre boşsa "" olur.
    /// </summary>
    public List<ExcelRow> ReadRows(IFormFile? file, IReadOnlyList<string> headers, int maxRows)
    {
        if (file is null || file.Length == 0)
            throw Errors.BadRequest("Excel dosyası seçilmelidir.");

        if (file.Length > MaxFileSize)
            throw Errors.BadRequest("Excel dosyası en fazla 5 MB olabilir.");

        if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            throw Errors.BadRequest("Sadece .xlsx uzantılı Excel dosyası yüklenebilir.");

        XLWorkbook workbook;
        try
        {
            using var stream = file.OpenReadStream();
            workbook = new XLWorkbook(stream);
        }
        catch (Exception)
        {
            throw Errors.BadRequest("Excel dosyası okunamadı. Şablonu indirip doldurduğunuz dosyayı yükleyin.");
        }

        using (workbook)
        {
            var sheet = workbook.Worksheets.First();
            var used = sheet.RangeUsed();
            if (used is null)
                throw Errors.BadRequest("Excel dosyası boş.");

            var headerRow = used.FirstRow();
            var columnOf = new Dictionary<string, int>();
            foreach (var cell in headerRow.Cells())
            {
                var text = cell.GetFormattedString().Trim();
                var header = headers.FirstOrDefault(h => string.Compare(h, text, Turkish, CompareOptions.IgnoreCase) == 0);
                if (header is not null)
                    columnOf.TryAdd(header, cell.Address.ColumnNumber);
            }

            var missing = headers.Where(h => !columnOf.ContainsKey(h)).ToList();
            if (missing.Count > 0)
                throw Errors.BadRequest($"Excel'de şu sütunlar bulunamadı: {string.Join(", ", missing)}. Şablondaki başlıkları kullanın.");

            var rows = new List<ExcelRow>();
            for (var rowNumber = headerRow.RowNumber() + 1; rowNumber <= used.LastRow().RowNumber(); rowNumber++)
            {
                var values = headers.ToDictionary(
                    h => h,
                    h => sheet.Cell(rowNumber, columnOf[h]).GetFormattedString().Trim());

                if (values.Values.All(string.IsNullOrEmpty))
                    continue;

                if (rows.Count == maxRows)
                    throw Errors.BadRequest($"Tek seferde en fazla {maxRows} satır eklenebilir.");

                rows.Add(new ExcelRow(rowNumber, values));
            }

            return rows;
        }
    }

    [GeneratedRegex(@"[\\/:*?""<>|]")]
    private static partial Regex InvalidFileNameChars();
}

/// <summary>Excel'den okunan satır. Number: Excel'deki satır numarası (hata mesajlarında gösterilir).</summary>
public sealed record ExcelRow(int Number, IReadOnlyDictionary<string, string> Values)
{
    public string this[string header] => Values[header];
}
