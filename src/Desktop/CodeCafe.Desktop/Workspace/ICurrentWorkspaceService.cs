namespace CodeCafe.Desktop.Workspace;

public interface ICurrentWorkspaceService
{
    Task<CurrentWorkspaceResponse> GetCurrentWorkspaceAsync(CancellationToken cancellationToken = default);
}
