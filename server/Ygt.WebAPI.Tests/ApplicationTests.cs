using System.Net;
using System.Net.Http.Json;

namespace Ygt.WebAPI.Tests;

[Collection(ApiCollection.Name)]
public sealed class ApplicationTests(ApiFactory factory)
{
    [Fact]
    public async Task Wrong_password_returns_turkish_message()
    {
        var response = await factory.CreateClient()
            .PostAsJsonAsync("/auth/login", new { email = ApiFactory.AdminEmail, password = "yanlis-sifre" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("E-posta veya şifre hatalı.", await response.MessageAsync());
    }

    [Fact]
    public async Task Course_application_is_validated_and_duplicates_are_rejected()
    {
        var admin = await factory.CreateAdminClientAsync();
        var course = await (await admin.PostAsJsonAsync("/courses", new { title = $"Test eğitimi {TestApi.Unique()}" }))
            .ReadAsync<IdBody>();

        var visitor = factory.CreateClient();
        var url = $"/courses/{course.Id}/applications";

        var first = await visitor.PostAsJsonAsync(url,
            new { firstName = "Ayşe", lastName = "Yılmaz", email = "Ayse@Test.com", phoneNumber = "0555 111 22 33" });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        // Aynı e-posta, harf büyüklüğü farklı olsa da tekrar sayılır.
        var duplicate = await visitor.PostAsJsonAsync(url,
            new { firstName = "Ayşe", lastName = "Yılmaz", email = "ayse@test.com" });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var invalidPhone = await visitor.PostAsJsonAsync(url,
            new { firstName = "Ali", lastName = "Can", email = "ali@test.com", phoneNumber = "123" });
        Assert.Equal(HttpStatusCode.BadRequest, invalidPhone.StatusCode);
        Assert.StartsWith("Telefon numarası geçerli değil", await invalidPhone.MessageAsync());
    }

    [Fact]
    public async Task Only_one_form_is_open_at_a_time()
    {
        var admin = await factory.CreateAdminClientAsync();
        var first = await (await admin.PostAsJsonAsync("/forms", new { title = $"Form A {TestApi.Unique()}" })).ReadAsync<IdBody>();
        var second = await (await admin.PostAsJsonAsync("/forms", new { title = $"Form B {TestApi.Unique()}" })).ReadAsync<IdBody>();

        // Yeni form açılınca önceki kapanır.
        Assert.Equal(second.Id, (await (await admin.GetAsync("/forms/active")).ReadAsync<IdBody>()).Id);

        // Kapalı form yeniden açılınca açık olan kapanır (filtreli unique index'e takılmadan).
        var reopen = await admin.PostAsync($"/forms/{first.Id}/reopen", null);
        Assert.Equal(HttpStatusCode.OK, reopen.StatusCode);
        Assert.Equal(first.Id, (await (await admin.GetAsync("/forms/active")).ReadAsync<IdBody>()).Id);
    }

    [Fact]
    public async Task Hackathon_member_cannot_join_two_teams()
    {
        var admin = await factory.CreateAdminClientAsync();
        var form = await (await admin.PostAsJsonAsync("/forms", new { title = $"Hackathon {TestApi.Unique()}", type = "hackathon" }))
            .ReadAsync<IdBody>();

        var visitor = factory.CreateClient();
        var url = $"/forms/{form.Id}/hackathon-applications";
        var captain = new { name = "Ali Can", phone = "0555 333 44 55", email = "ali@ogr.ktun.edu.tr" };

        var firstTeam = await visitor.PostAsJsonAsync(url, new
        {
            teamName = "Kod Kedileri",
            member1 = captain,
            member2 = new { name = "Zeynep Su", phone = "5554445566", email = "zeynep@ogr.ktun.edu.tr" }
        });
        Assert.Equal(HttpStatusCode.OK, firstTeam.StatusCode);

        var secondTeam = await visitor.PostAsJsonAsync(url, new
        {
            teamName = "Bit Avcıları",
            member1 = captain,
            member2 = new { name = "Su Ak", phone = "5550000002", email = "su@ogr.ktun.edu.tr" }
        });
        Assert.Equal(HttpStatusCode.Conflict, secondTeam.StatusCode);
        Assert.Contains("Ali Can", await secondTeam.MessageAsync());
    }
}
