using System.Globalization;
using CodeCafe.Desktop.Workspace;

namespace CodeCafe.Desktop.ViewModels;

public sealed class WorkspaceShellViewModel : ViewModelBase
{
    private readonly ICurrentWorkspaceService currentWorkspaceService;
    private string currentWorkspaceDescription = "Preparing workspace context.";
    private string currentWorkspaceDisplayName = "Loading workspace";
    private string currentWorkspaceId = "pending";
    private string lastLoadedAt = "Not loaded";
    private string selectedNavigationItem = "Workspace";
    private string workspaceLoadStatus = "Loading";

    public WorkspaceShellViewModel(ICurrentWorkspaceService currentWorkspaceService)
    {
        this.currentWorkspaceService = currentWorkspaceService;
    }

    public IReadOnlyList<string> NavigationItems { get; } =
    [
        "Workspace",
        "Notes",
        "Code",
        "AI",
        "Activity",
    ];

    public string CurrentWorkspaceDisplayName
    {
        get => currentWorkspaceDisplayName;
        private set => SetProperty(ref currentWorkspaceDisplayName, value);
    }

    public string CurrentWorkspaceId
    {
        get => currentWorkspaceId;
        private set => SetProperty(ref currentWorkspaceId, value);
    }

    public string CurrentWorkspaceDescription
    {
        get => currentWorkspaceDescription;
        private set => SetProperty(ref currentWorkspaceDescription, value);
    }

    public string LastLoadedAt
    {
        get => lastLoadedAt;
        private set => SetProperty(ref lastLoadedAt, value);
    }

    public string SelectedNavigationItem
    {
        get => selectedNavigationItem;
        set => SetProperty(ref selectedNavigationItem, value);
    }

    public string WorkspaceLoadStatus
    {
        get => workspaceLoadStatus;
        private set => SetProperty(ref workspaceLoadStatus, value);
    }

    public async Task LoadCurrentWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        WorkspaceLoadStatus = "Loading";

        try
        {
            CurrentWorkspaceResponse workspace =
                await currentWorkspaceService.GetCurrentWorkspaceAsync(cancellationToken).ConfigureAwait(true);

            CurrentWorkspaceDisplayName = workspace.DisplayName;
            CurrentWorkspaceId = workspace.Id.ToString();
            CurrentWorkspaceDescription = workspace.Description ?? "No workspace description available.";
            LastLoadedAt = workspace.LoadedAt.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
            WorkspaceLoadStatus = "Current";
        }
        catch (OperationCanceledException)
        {
            WorkspaceLoadStatus = "Canceled";
        }
        catch (InvalidOperationException ex)
        {
            CurrentWorkspaceDescription = ex.Message;
            WorkspaceLoadStatus = "Unavailable";
        }
    }
}
