namespace CodeCafe.Modules.Platform.Contracts.Auth;

/// <summary>
/// HTTP request shape for <c>POST /api/auth/register</c>. Kept as a record
/// so the Web adapter can pattern-match on it without ceremony.
/// </summary>
public sealed record RegisterRequest(string Email, string Password, string? DisplayName);
