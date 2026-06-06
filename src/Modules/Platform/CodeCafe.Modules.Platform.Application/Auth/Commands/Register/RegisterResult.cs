namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Register;

/// <summary>
/// Outcome of a successful registration. The returned id is enough to log
/// the user in or render a confirmation; we keep it small and stable.
/// </summary>
public sealed record RegisterResult(Guid UserId, string Email, string? DisplayName);
