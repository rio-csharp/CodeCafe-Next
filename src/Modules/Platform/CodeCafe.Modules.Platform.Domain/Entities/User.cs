using CodeCafe.Modules.Platform.Domain.ValueObjects;

namespace CodeCafe.Modules.Platform.Domain.Entities;

/// <summary>
/// Platform-level user account. Intentionally minimal: enough to identify a
/// human or service principal that authenticates against the workspace.
///
/// Passwords are stored only as an opaque hash. Hashing policy is owned by
/// the application / infrastructure layers, never by the domain entity.
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }

    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Opaque password hash produced by the application-layer hasher.
    /// The domain never interprets this value.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    public string? DisplayName { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    // EF Core needs a parameterless constructor for materialisation.
#pragma warning disable CS8618
    private User() { }
#pragma warning restore CS8618

    private User(Guid id, Email email, string passwordHash, string? displayName, DateTime createdAtUtc)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Factory used by the registration use case. Validates the inputs only at
    /// the level the domain can defend: non-null, non-empty hash. Callers
    /// (application layer) are responsible for hashing the plaintext.
    /// </summary>
    public static User Register(Email email, string passwordHash, string? displayName, DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new User(Guid.NewGuid(), email, passwordHash, displayName, createdAtUtc);
    }
}
