namespace CodeCafe.Modules.Platform.Application.Auth.Commands.Login;

/// <summary>
/// Identity returned by a successful login. The application layer does not
/// decide how the caller persists this (cookie, token, ...); it just hands
/// the data back to the adapter.
/// </summary>
public sealed record LoginResult(Guid UserId, string Email, string? DisplayName);
