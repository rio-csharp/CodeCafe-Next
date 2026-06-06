using CodeCafe.Modules.Platform.Domain.Enums;
using CodeCafe.Modules.Platform.Domain.ValueObjects;

namespace CodeCafe.Modules.Platform.Domain.Entities;

/// <summary>
/// Platform-owned workspace. The first foundation supports a single default
/// personal workspace owned by one authenticated user.
/// </summary>
public sealed class Workspace
{
    public const string DefaultPersonalWorkspaceName = "Personal workspace";

    public WorkspaceId Id { get; private set; }

    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public WorkspaceKind Kind { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    // EF Core needs a parameterless constructor for materialisation.
#pragma warning disable CS8618
    private Workspace() { }
#pragma warning restore CS8618

    private Workspace(
        WorkspaceId id,
        Guid ownerUserId,
        string name,
        WorkspaceKind kind,
        DateTime createdAtUtc)
    {
        Id = id;
        OwnerUserId = ownerUserId;
        Name = name;
        Kind = kind;
        CreatedAtUtc = createdAtUtc;
    }

    public static Workspace CreateDefaultPersonal(Guid ownerUserId, DateTime createdAtUtc)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("Workspace owner is required.", nameof(ownerUserId));
        }

        return new Workspace(
            WorkspaceId.New(),
            ownerUserId,
            DefaultPersonalWorkspaceName,
            WorkspaceKind.Personal,
            createdAtUtc);
    }
}
