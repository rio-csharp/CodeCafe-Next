namespace CodeCafe.Modules.Platform.Contracts.Auth;

/// <summary>
/// HTTP request shape for <c>POST /api/auth/login</c>.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
