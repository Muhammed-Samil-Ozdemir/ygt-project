using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ygt.WebAPI.Context;

namespace Ygt.WebAPI.Tests;

[Collection(ApiCollection.Name)]
public sealed class SiteContentTests(ApiFactory factory)
{
    [Fact]
    public async Task First_install_comes_with_default_content_but_no_sample_data()
    {
        var client = factory.CreateClient();
        var content = await (await client.GetAsync("/site/content")).ReadAsync<SiteContentBody>();

        Assert.Equal(DefaultContent.Texts["siteName"], content.Texts["siteName"]);
        Assert.Equal(DefaultContent.Texts["kvkkText"].ReplaceLineEndings("\n"), content.Texts["kvkkText"]);
        Assert.Equal(DefaultContent.Technologies, content.Technologies);
        Assert.Equal(DefaultContent.About.Length, content.About.Count);
        Assert.Equal(DefaultContent.Links.Length, content.Links.Count);

        // Örnek veri ayarı testlerde kapalı.
        Assert.Empty(content.Team);
        Assert.Empty(content.Events);

        // Logo SeedData'dan storage'a kopyalanmış olmalı.
        var logo = await client.GetAsync($"/site/content/images/{content.Texts["logo"]}");
        Assert.Equal(HttpStatusCode.OK, logo.StatusCode);
        Assert.Equal("image/png", logo.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Cleared_text_stays_cleared_after_restart()
    {
        var admin = await factory.CreateAdminClientAsync();
        var cleared = await admin.PutAsJsonAsync("/site/content/texts",
            new { values = new Dictionary<string, string?> { ["heroEyebrow"] = "" } });
        cleared.EnsureSuccessStatusCode();

        // Yeni bir uygulama örneği aynı veritabanıyla açılır; başlangıç içeriği tekrar eklenmemeli.
        await using var restarted = factory.WithWebHostBuilder(_ => { });
        var content = await (await restarted.CreateClient().GetAsync("/site/content")).ReadAsync<SiteContentBody>();

        Assert.Equal("", content.Texts["heroEyebrow"]);
        Assert.Equal(DefaultContent.Technologies, content.Technologies);
    }

    private sealed record SiteContentBody(
        Dictionary<string, string> Texts,
        List<JsonElement> About,
        List<JsonElement> Events,
        List<JsonElement> Team,
        List<string> Technologies,
        List<JsonElement> Links);
}
