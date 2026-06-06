using MediatR;

namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Register;

/// <summary>
/// Register a new platform user. The command carries only data; the handler
/// does password hashing, persistence, and any duplication checks.
/// </summary>
public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName) : IRequest<RegisterResult>;
