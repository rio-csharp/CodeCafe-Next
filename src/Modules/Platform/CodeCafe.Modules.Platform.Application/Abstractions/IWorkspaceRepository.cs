using CodeCafe.Modules.Platform.Domain.Entities;

namespace CodeCafe.Modules.Platform.Application.Abstractions;

/// <summary>
/// Module-specific workspace persistence contract. It exposes only the
/// personal-workspace operations needed by the current Platform foundation.
/// </summary>
public interface IWorkspaceRepository
{
    Task<Workspace?> FindDefaultPersonalForOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken);

    Task<Workspace> EnsureDefaultPersonalForOwnerAsync(
        Guid ownerUserId,
        DateTime createdAtUtc,
        CancellationToken cancellationToken);
}
