namespace CodeCafe.Desktop.Workspace;

public interface ICurrentWorkspaceClient
{
    Task<CurrentWorkspaceResponse> GetCurrentWorkspaceAsync(CancellationToken cancellationToken = default);
}
