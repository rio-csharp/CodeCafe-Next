using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Application.Exceptions;
using CodeCafe.Modules.Platform.Domain.Entities;
using CodeCafe.Modules.Platform.Domain.ValueObjects;
using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Register;

/// <summary>
/// Validates the email, hashes the password, persists the user, and returns
/// a small result. The handler does not touch transport concerns.
/// </summary>
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClock _clock;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IWorkspaceRepository workspaceRepository,
        IPasswordHasher passwordHasher,
        IClock clock)
    {
        _userRepository = userRepository;
        _workspaceRepository = workspaceRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
        {
            throw new EmailAlreadyExistsException();
        }

        var hash = _passwordHasher.Hash(request.Password);
        var user = User.Register(email, hash, request.DisplayName, _clock.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);
        await _workspaceRepository.AddAsync(
            Workspace.CreateDefaultPersonal(user.Id, _clock.UtcNow),
            cancellationToken);

        return new RegisterResult(user.Id, user.Email.Value, user.DisplayName);
    }
}
