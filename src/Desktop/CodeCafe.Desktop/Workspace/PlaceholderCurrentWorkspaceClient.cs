namespace CodeCafe.Desktop.Workspace;

public sealed class PlaceholderCurrentWorkspaceClient : ICurrentWorkspaceClient
{
    public Task<CurrentWorkspaceResponse> GetCurrentWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var workspace = new CurrentWorkspaceResponse(
            new WorkspaceId("local-placeholder-workspace"),
            "CodeCafe Local Workspace",
            "Local desktop placeholder data until the Platform current workspace contract is merged.",
            DateTimeOffset.UtcNow);

        return Task.FromResult(workspace);
    }
}
