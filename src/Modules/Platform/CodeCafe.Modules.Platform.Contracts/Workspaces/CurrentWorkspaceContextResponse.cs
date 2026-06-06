namespace CodeCafe.Modules.Platform.Contracts.Workspaces;

/// <summary>
/// Transport contract for the authenticated caller's current workspace
/// context.
/// </summary>
public sealed record CurrentWorkspaceContextResponse(Guid UserId, WorkspaceResponse Workspace);
