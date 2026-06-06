namespace CodeCafe.Modules.Platform.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Application-layer read model for the authenticated user. Distinct from
/// the persistence entity so the Web adapter never has to expose it.
/// </summary>
public sealed record CurrentUserView(Guid UserId, string Email, string? DisplayName);
