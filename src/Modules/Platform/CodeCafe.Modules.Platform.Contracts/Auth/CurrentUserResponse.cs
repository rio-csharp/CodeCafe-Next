namespace CodeCafe.Modules.Platform.Contracts.Auth;

/// <summary>
/// HTTP response shape for <c>GET /api/auth/me</c>.
/// </summary>
public sealed record CurrentUserResponse(Guid UserId, string Email, string? DisplayName);
