using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ygt.WebAPI.Tests;

/// <summary>Testlerde tekrar eden API çağrıları.</summary>
internal static class TestApi
{
    /// <summary>Giriş yapmış yönetici gibi istek atan istemci.</summary>
    public static async Task<HttpClient> CreateAdminClientAsync(
        this ApiFactory factory,
        string email = ApiFactory.AdminEmail,
        string password = ApiFactory.AdminPassword)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await client.LoginAsync(email, password));
        return client;
    }

    public static async Task<string> LoginAsync(this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TokenBody>())!.Token;
    }

    /// <summary>API her hatada { message } döndürür.</summary>
    public static async Task<string> MessageAsync(this HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<MessageBody>())!.Message;

    public static async Task<T> ReadAsync<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    /// <summary>Testler aynı veritabanını paylaştığı için başlık ve e-postalar her testte farklı olsun.</summary>
    public static string Unique() => Guid.NewGuid().ToString("N")[..8];
}

internal sealed record TokenBody(string Token);
internal sealed record MessageBody(string Message);
internal sealed record IdBody(Guid Id);
