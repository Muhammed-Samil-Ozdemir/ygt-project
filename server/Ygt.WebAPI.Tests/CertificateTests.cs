using System.Net;
using System.Net.Http.Json;
using ClosedXML.Excel;

namespace Ygt.WebAPI.Tests;

[Collection(ApiCollection.Name)]
public sealed class CertificateTests(ApiFactory factory)
{
    [Fact]
    public async Task Excel_import_adds_nothing_if_any_row_is_invalid()
    {
        var admin = await factory.CreateAdminClientAsync();
        var course = $"Excel testi {TestApi.Unique()}";
        var before = await CountAsync(admin);

        var invalid = await UploadAsync(admin,
            [course, "Ayşe", "Yılmaz", "", ""],
            [course, "", "Kaya", "", ""]);

        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.StartsWith("3. satır: Ad zorunludur.", await invalid.MessageAsync());
        Assert.Equal(before, await CountAsync(admin));

        var valid = await UploadAsync(admin,
            [course, "Ayşe", "Yılmaz", "ayse@test.com", "0555 111 22 33"],
            [course, "Mehmet", "Kaya", "", ""]);

        Assert.Equal("2 sertifika eklendi.", (await valid.ReadAsync<MessageBody>()).Message);
        Assert.Equal(before + 2, await CountAsync(admin));
    }

    [Fact]
    public async Task Export_contains_verification_links()
    {
        var admin = await factory.CreateAdminClientAsync();
        var created = await (await admin.PostAsJsonAsync("/certificates",
            new { courseName = $"Link testi {TestApi.Unique()}", firstName = "Ece", lastName = "Naz" })).ReadAsync<CodeBody>();

        var response = await admin.GetAsync("/certificates/export?siteUrl=https%3A%2F%2Fygt.example.com%2F");
        response.EnsureSuccessStatusCode();

        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        var links = workbook.Worksheet(1).CellsUsed().Select(c => c.GetString()).ToList();
        Assert.Contains($"https://ygt.example.com/certificates/{created.Code}", links);
    }

    private static async Task<int> CountAsync(HttpClient admin) =>
        (await (await admin.GetAsync("/certificates")).ReadAsync<List<IdBody>>()).Count;

    /// <summary>Şablondaki başlıklarla bir Excel oluşturup toplu ekleme ucuna yükler.</summary>
    private static async Task<HttpResponseMessage> UploadAsync(HttpClient admin, params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Sertifikalar");
        string[] headers = ["Eğitim adı", "Ad", "Soyad", "E-posta", "Telefon"];

        for (var column = 0; column < headers.Length; column++)
            sheet.Cell(1, column + 1).Value = headers[column];

        for (var row = 0; row < rows.Length; row++)
            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(row + 2, column + 1).Value = rows[row][column];

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(stream.ToArray()), "file", "sertifikalar.xlsx");
        return await admin.PostAsync("/certificates/import", content);
    }

    private sealed record CodeBody(string Code);
}
