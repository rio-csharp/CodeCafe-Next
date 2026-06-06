using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CodeCafe.Desktop.ViewModels;
using CodeCafe.Desktop.Views;
using CodeCafe.Desktop.Workspace;

namespace CodeCafe.Desktop;

public sealed partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ICurrentWorkspaceClient workspaceClient = new PlaceholderCurrentWorkspaceClient();
            ICurrentWorkspaceService workspaceService = new CurrentWorkspaceService(workspaceClient);

            var shellViewModel = new WorkspaceShellViewModel(workspaceService);
            desktop.MainWindow = new MainWindow
            {
                DataContext = shellViewModel,
            };

            _ = shellViewModel.LoadCurrentWorkspaceAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
