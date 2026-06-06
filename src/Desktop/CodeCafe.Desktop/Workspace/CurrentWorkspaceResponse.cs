namespace CodeCafe.Desktop.Workspace;

public sealed record CurrentWorkspaceResponse(
    WorkspaceId Id,
    string DisplayName,
    string? Description,
    DateTimeOffset LoadedAt);
