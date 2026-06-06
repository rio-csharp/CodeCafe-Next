using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of the Platform workspace repository.
/// </summary>
public sealed class WorkspaceRepository : IWorkspaceRepository
{
    private readonly PlatformDbContext _db;

    public WorkspaceRepository(PlatformDbContext db) => _db = db;

    public Task<Workspace?> FindDefaultPersonalForOwnerAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken) =>
        _db.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(
                w => w.OwnerUserId == ownerUserId && w.Kind == WorkspaceKind.Personal,
                cancellationToken);

    public async Task<Workspace> EnsureDefaultPersonalForOwnerAsync(
        Guid ownerUserId,
        DateTime createdAtUtc,
        CancellationToken cancellationToken)
    {
        var existing = await FindDefaultPersonalForOwnerAsync(ownerUserId, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var workspace = Workspace.CreateDefaultPersonal(ownerUserId, createdAtUtc);
        await _db.Workspaces.AddAsync(workspace, cancellationToken);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return workspace;
        }
        catch (DbUpdateException)
        {
            _db.Entry(workspace).State = EntityState.Detached;

            var concurrentWinner = await FindDefaultPersonalForOwnerAsync(
                ownerUserId,
                cancellationToken);
            if (concurrentWinner is not null)
            {
                return concurrentWinner;
            }

            throw;
        }
    }
}
