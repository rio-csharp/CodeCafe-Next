namespace CodeCafe.Modules.Platform.Contracts.Workspaces;

/// <summary>
/// Basic workspace response DTO for Platform clients.
/// </summary>
public sealed record WorkspaceResponse(
    Guid WorkspaceId,
    string Name,
    string Kind,
    Guid OwnerUserId,
    DateTime CreatedAtUtc);
