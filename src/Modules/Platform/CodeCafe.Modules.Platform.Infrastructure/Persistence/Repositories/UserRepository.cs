using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of the module-specific user repository. The only
/// reason this class exists is to keep EF Core out of the application layer
/// and the Web adapter.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly PlatformDbContext _db;

    public UserRepository(PlatformDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken) =>
        _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
