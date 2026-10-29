using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ygt.WebAPI.Tests;

[Collection(ApiCollection.Name)]
public sealed class UserTests(ApiFactory factory)
{
    [Fact]
    public async Task Deleted_admin_token_is_rejected_immediately()
    {
        var admin = await factory.CreateAdminClientAsync();
        var (id, email, password) = await CreateUserAsync(admin);
        var other = await factory.CreateAdminClientAsync(email, password);

        Assert.Equal(HttpStatusCode.OK, (await other.GetAsync("/users")).StatusCode);

        await admin.DeleteAsync($"/users/{id}");

        // Token'ın süresi dolmamış olsa da silinen yönetici artık istek atamaz.
        Assert.Equal(HttpStatusCode.Unauthorized, (await other.GetAsync("/users")).StatusCode);
    }

    [Fact]
    public async Task Admin_cannot_delete_own_account()
    {
        var admin = await factory.CreateAdminClientAsync();
        var me = await (await admin.GetAsync("/auth/me")).ReadAsync<IdBody>();

        var response = await admin.DeleteAsync($"/users/{me.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Kendi hesabınızı silemezsiniz.", await response.MessageAsync());
    }

    [Fact]
    public async Task Changing_own_password_requires_current_password()
    {
        var admin = await factory.CreateAdminClientAsync();
        var (_, email, password) = await CreateUserAsync(admin);
        var user = await factory.CreateAdminClientAsync(email, password);

        var wrong = await user.PutAsJsonAsync("/auth/password", new { currentPassword = "yanlis-sifre", newPassword = "Yeni_Sifre_2026" });
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);

        var changed = await user.PutAsJsonAsync("/auth/password", new { currentPassword = password, newPassword = "Yeni_Sifre_2026" });
        Assert.Equal(HttpStatusCode.OK, changed.StatusCode);

        var client = factory.CreateClient();
        var oldLogin = await client.PostAsJsonAsync("/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);
        Assert.NotEmpty(await client.LoginAsync(email, "Yeni_Sifre_2026"));
    }

    private static async Task<(Guid Id, string Email, string Password)> CreateUserAsync(HttpClient admin)
    {
        var email = $"yonetici-{TestApi.Unique()}@test.local";
        const string password = "Ilk_Sifre_2026";

        var created = await (await admin.PostAsJsonAsync("/users", new { name = "Test Yönetici", email, password }))
            .ReadAsync<IdBody>();

        return (created.Id, email, password);
    }
}
