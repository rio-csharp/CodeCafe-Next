namespace CodeCafe.Modules.Platform.Application.Abstractions;

/// <summary>
/// Read-only view of the caller inside a request scope. The application
/// layer never reaches into <c>HttpContext</c> directly; adapters and
/// infrastructure provide an implementation of this interface so handlers
/// stay transport-agnostic.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Identifier of the authenticated user, or <c>null</c> when the
    /// request is anonymous. Treat absence as "no current user" rather
    /// than a special "guest" id.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Email of the authenticated user, when available.
    /// </summary>
    string? Email { get; }

    /// <summary>True if the request has an authenticated identity.</summary>
    bool IsAuthenticated { get; }
}
