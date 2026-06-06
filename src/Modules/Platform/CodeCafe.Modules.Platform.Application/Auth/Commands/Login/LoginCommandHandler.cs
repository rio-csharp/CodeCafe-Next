using CodeCafe.Modules.Platform.Application.Abstractions;
using CodeCafe.Modules.Platform.Application.Exceptions;
using CodeCafe.Modules.Platform.Domain.ValueObjects;
using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Login;

/// <summary>
/// Verifies credentials against stored hashes. Failure paths collapse to a
/// single <see cref="InvalidCredentialsException"/> so the adapter can map
/// both "unknown email" and "bad password" to the same 401 response.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.FindByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return new LoginResult(user.Id, user.Email.Value, user.DisplayName);
    }
}
