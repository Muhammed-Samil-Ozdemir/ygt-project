using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Ygt.WebAPI.Dtos;

namespace Ygt.WebAPI.Providers;

/// <summary>Başvuru listelerini Excel (.xlsx) dosyasına çevirir.</summary>
public sealed partial class ExcelProvider
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    /// <param name="title">Dosya adında kullanılır, ör. "Hackathon 2026" -> "Hackathon 2026 - başvurular.xlsx".</param>
    /// <param name="columns">Sütun başlığı ve her satır için o sütunun değeri.</param>
    public FileDto Create<T>(string title, IReadOnlyList<T> rows, params (string Header, Func<T, string?> Value)[] columns)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Başvurular");

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

        var fileName = $"{InvalidFileNameChars().Replace(title, "")} - başvurular.xlsx";
        return new FileDto(stream.ToArray(), ContentType, fileName);
    }

    [GeneratedRegex(@"[\\/:*?""<>|]")]
    private static partial Regex InvalidFileNameChars();
}
