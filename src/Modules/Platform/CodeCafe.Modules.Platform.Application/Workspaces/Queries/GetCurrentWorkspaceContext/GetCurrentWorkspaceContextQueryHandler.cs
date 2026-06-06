using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Domain.Entities;
using MediatR;

namespace CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;

/// <summary>
/// Resolves the caller's default personal workspace without taking a
/// dependency on HTTP, controller endpoints, or host wiring.
/// </summary>
public sealed class GetCurrentWorkspaceContextQueryHandler
    : IRequestHandler<GetCurrentWorkspaceContextQuery, CurrentWorkspaceContextView?>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IClock _clock;

    public GetCurrentWorkspaceContextQueryHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IWorkspaceRepository workspaceRepository,
        IClock clock)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _workspaceRepository = workspaceRepository;
        _clock = clock;
    }

    public async Task<CurrentWorkspaceContextView?> Handle(
        GetCurrentWorkspaceContextQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not { } userId)
        {
            return null;
        }

        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var workspace = await _workspaceRepository.FindDefaultPersonalForOwnerAsync(
            user.Id,
            cancellationToken);
        if (workspace is null)
        {
            workspace = Workspace.CreateDefaultPersonal(user.Id, _clock.UtcNow);
            await _workspaceRepository.AddAsync(workspace, cancellationToken);
        }

        return new CurrentWorkspaceContextView(
            user.Id,
            new WorkspaceView(
                workspace.Id.Value,
                workspace.Name,
                workspace.Kind.ToString(),
                workspace.OwnerUserId,
                workspace.CreatedAtUtc));
    }
}
