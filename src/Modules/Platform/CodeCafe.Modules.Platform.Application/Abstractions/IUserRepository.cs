using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.ValueObjects;

namespace CodeCafe.Modules.Platform.Application.Abstractions;

/// <summary>
/// Module-specific user persistence contract. Not a generic repository:
/// it exposes only the operations the application actually performs.
/// </summary>
public interface IUserRepository
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken);

    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);

    Task AddWithDefaultWorkspaceAsync(
        User user,
        Workspace workspace,
        CancellationToken cancellationToken);
}
