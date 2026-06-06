using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Login;

/// <summary>
/// Authenticate a user by email + password. The handler returns the user
/// identity on success; the adapter is responsible for translating that into
/// a transport artefact (cookie, token, etc.).
/// </summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
