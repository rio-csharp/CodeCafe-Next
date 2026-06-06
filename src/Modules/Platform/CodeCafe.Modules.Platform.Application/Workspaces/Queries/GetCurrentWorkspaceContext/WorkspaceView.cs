namespace CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;

/// <summary>
/// Application-layer read model for a workspace. Kept separate from the
/// domain entity and transport contracts.
/// </summary>
public sealed record WorkspaceView(
    Guid WorkspaceId,
    string Name,
    string Kind,
    Guid OwnerUserId,
    DateTime CreatedAtUtc);
