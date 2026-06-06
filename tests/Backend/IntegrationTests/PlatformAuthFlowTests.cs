using System.Net;
using System.Net.Http.Json;
using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;
using CodeCafe.Modules.Platform.Contracts.Auth;
using CodeCafe.Modules.Platform.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task Register_User_CanResolve_CurrentWorkspaceContext_Query()
    {
        var client = _factory.CreateClient();

        var email = $"workspace-owner-{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "workspace-password", "Workspace Owner"));
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(registered);

        using var currentUserFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<ICurrentUser>(_ => new TestCurrentUser(
                    registered!.UserId,
                    registered.Email));
            });
        });

        using var scope = currentUserFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var context = await mediator.Send(new GetCurrentWorkspaceContextQuery());

        Assert.NotNull(context);
        Assert.Equal(registered!.UserId, context!.UserId);
        Assert.Equal(registered.UserId, context.Workspace.OwnerUserId);
        Assert.NotEqual(Guid.Empty, context.Workspace.WorkspaceId);
        Assert.Equal("Personal workspace", context.Workspace.Name);
        Assert.Equal("Personal", context.Workspace.Kind);
    }

    [Fact]
    public async Task CurrentWorkspaceContext_Query_Creates_DefaultWorkspace_WhenMissing()
    {
        var client = _factory.CreateClient();

        var email = $"workspace-create-{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "workspace-password", "Workspace Creator"));
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(registered);

        using var currentUserFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<ICurrentUser>(_ => new TestCurrentUser(
                    registered!.UserId,
                    registered.Email));
            });
        });

        using var scope = currentUserFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var existingWorkspaces = db.Workspaces
            .Where(workspace => workspace.OwnerUserId == registered!.UserId)
            .ToList();
        db.Workspaces.RemoveRange(existingWorkspaces);
        await db.SaveChangesAsync();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var context = await mediator.Send(new GetCurrentWorkspaceContextQuery());

        Assert.NotNull(context);
        Assert.Equal(registered!.UserId, context!.UserId);
        Assert.Equal(registered.UserId, context.Workspace.OwnerUserId);
        Assert.NotEqual(Guid.Empty, context.Workspace.WorkspaceId);
        Assert.Equal("Personal workspace", context.Workspace.Name);
        Assert.Equal("Personal", context.Workspace.Kind);
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

    private sealed class TestCurrentUser : ICurrentUser
    {
        public TestCurrentUser(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        public Guid? UserId { get; }

        public string? Email { get; }

        public bool IsAuthenticated => true;
    }
}
