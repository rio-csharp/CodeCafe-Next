using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Returns a stable view of the caller. The query deliberately does not
/// throw when anonymous: the adapter turns a null result into a 401.
/// </summary>
public sealed record GetCurrentUserQuery : IRequest<CurrentUserView?>;
