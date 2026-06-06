namespace CodeCafe.Modules.Platform.Contracts.Auth;

/// <summary>
/// HTTP response shape for register / login. Same payload on both endpoints:
/// clients only need to know "here is the identity you have now".
/// </summary>
public sealed record AuthResponse(Guid UserId, string Email, string? DisplayName);
