using System.Net;
using System.Net.Http.Json;
using CodeCafe.Modules.Platform.Contracts.Auth;

namespace CodeCafe.IntegrationTests;

/// <summary>
/// End-to-end test for the register -> login -> me chain. The Host's
/// composition root is booted in-process via <see cref="PlatformAuthWebApplicationFactory"/>;
/// each case uses a unique email so the per-instance SQLite database stays
/// self-consistent.
/// </summary>
public sealed class PlatformAuthFlowTests : IClassFixture<PlatformAuthWebApplicationFactory>
{
    private readonly PlatformAuthWebApplicationFactory _factory;

    public PlatformAuthFlowTests(PlatformAuthWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Me_WithoutCookie_Returns_401()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = false
        });

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_Login_Me_ProtectedPing_Roundtrip_Succeeds()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        var email = $"alice-{Guid.NewGuid():N}@example.com";
        var password = "sup3r-secret";

        // 1. Register.
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password, "Alice"));
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(registered);
        Assert.Equal(email, registered!.Email);
        Assert.Equal("Alice", registered.DisplayName);
        Assert.NotEqual(Guid.Empty, registered.UserId);

        // 2. Login (should set the cookie).
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        // 3. /me reads the cookie and resolves the same identity.
        var meResponse = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var me = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();
        Assert.NotNull(me);
        Assert.Equal(registered.UserId, me!.UserId);
        Assert.Equal(email, me.Email);

        // 4. The protected /ping endpoint proves the cookie ticket survives
        //    the ICurrentUser abstraction reaching the controller.
        var pingResponse = await client.GetAsync("/api/protected/ping");
        Assert.Equal(HttpStatusCode.OK, pingResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns_401_With_Stable_Code()
    {
        var client = _factory.CreateClient();

        var email = $"bob-{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "good-password", "Bob"));

        var badLogin = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(email, "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, badLogin.StatusCode);
    }

    [Fact]
    public async Task DuplicateRegistration_Returns_409()
    {
        var client = _factory.CreateClient();

        var email = $"carol-{Guid.NewGuid():N}@example.com";
        var first = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "pw", "Carol"));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "pw", "Carol"));
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
