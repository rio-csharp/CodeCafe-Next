using CodeCafe.Modules.Platform.Application.Abstractions;
using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Resolves the current caller's identity from the abstraction. If the
/// request is anonymous or the referenced user no longer exists, the query
/// returns <c>null</c>; the adapter decides how to translate that.
/// </summary>
public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserView?>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(ICurrentUser currentUser, IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<CurrentUserView?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not { } userId)
        {
            return null;
        }

        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        return user is null
            ? null
            : new CurrentUserView(user.Id, user.Email.Value, user.DisplayName);
    }
}
