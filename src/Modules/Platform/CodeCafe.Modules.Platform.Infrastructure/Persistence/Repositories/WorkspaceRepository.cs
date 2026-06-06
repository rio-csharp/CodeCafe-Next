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

    public async Task AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        await _db.Workspaces.AddAsync(workspace, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
