namespace CodeCafe.Desktop.Workspace;

public sealed class CurrentWorkspaceService : ICurrentWorkspaceService
{
    private readonly ICurrentWorkspaceClient client;

    public CurrentWorkspaceService(ICurrentWorkspaceClient client)
    {
        this.client = client;
    }

    public Task<CurrentWorkspaceResponse> GetCurrentWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        return client.GetCurrentWorkspaceAsync(cancellationToken);
    }
}
