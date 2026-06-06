using System.Net;
using System.Net.Http.Json;
using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;
using CodeCafe.Modules.Platform.Contracts.Auth;
using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.ValueObjects;
using CodeCafe.Modules.Platform.Infrastructure.Persistence;
using CodeCafe.Modules.Platform.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
    public async Task Register_Persistence_RollsBack_User_When_DefaultWorkspace_Insert_Fails()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var repository = new UserRepository(db);

        var user = User.Register(
            Email.Create($"atomic-{Guid.NewGuid():N}@example.com"),
            "opaque-test-hash",
            "Atomic Owner",
            DateTime.UtcNow);
        var workspaceWithMissingOwner = Workspace.CreateDefaultPersonal(
            Guid.NewGuid(),
            DateTime.UtcNow);

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            repository.AddWithDefaultWorkspaceAsync(
                user,
                workspaceWithMissingOwner,
                CancellationToken.None));

        db.ChangeTracker.Clear();
        var userWasPersisted = await db.Users.AnyAsync(
            candidate => candidate.Id == user.Id,
            CancellationToken.None);
        Assert.False(userWasPersisted);
    }

    [Fact]
    public async Task CurrentWorkspaceContext_Query_Handles_Concurrent_DefaultWorkspace_Creation()
    {
        var client = _factory.CreateClient();

        var email = $"workspace-concurrent-{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, "workspace-password", "Workspace Racer"));
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

        using (var setupScope = currentUserFactory.Services.CreateScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<PlatformDbContext>();
            var existingWorkspaces = await db.Workspaces
                .Where(workspace => workspace.OwnerUserId == registered!.UserId)
                .ToListAsync();
            db.Workspaces.RemoveRange(existingWorkspaces);
            await db.SaveChangesAsync();
        }

        async Task<CurrentWorkspaceContextView?> ResolveContextAsync()
        {
            using var queryScope = currentUserFactory.Services.CreateScope();
            var mediator = queryScope.ServiceProvider.GetRequiredService<IMediator>();
            return await mediator.Send(new GetCurrentWorkspaceContextQuery());
        }

        var contexts = await Task.WhenAll(
            Enumerable.Range(0, 8).Select(_ => ResolveContextAsync()));

        Assert.All(contexts, context =>
        {
            Assert.NotNull(context);
            Assert.Equal(registered!.UserId, context!.UserId);
            Assert.Equal(registered.UserId, context.Workspace.OwnerUserId);
        });

        var workspaceIds = contexts
            .Select(context => context!.Workspace.WorkspaceId)
            .Distinct()
            .ToArray();
        Assert.Single(workspaceIds);

        using var verifyScope = currentUserFactory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var workspaceCount = await verifyDb.Workspaces.CountAsync(
            workspace => workspace.OwnerUserId == registered!.UserId);
        Assert.Equal(1, workspaceCount);
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
