using MediatR;

namespace CodeCafe.Modules.Platform.Application.Workspaces.Queries.GetCurrentWorkspaceContext;

/// <summary>
/// Returns the authenticated caller's current Platform workspace context.
/// Anonymous callers or deleted users resolve to <c>null</c>; authenticated
/// users without a workspace get a default personal workspace.
/// </summary>
public sealed record GetCurrentWorkspaceContextQuery : IRequest<CurrentWorkspaceContextView?>;
