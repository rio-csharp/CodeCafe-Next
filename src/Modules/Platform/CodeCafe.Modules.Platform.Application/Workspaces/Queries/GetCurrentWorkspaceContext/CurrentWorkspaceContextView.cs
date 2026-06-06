namespace CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;

/// <summary>
/// Application-layer read model for the current workspace context.
/// </summary>
public sealed record CurrentWorkspaceContextView(Guid UserId, WorkspaceView Workspace);
