namespace CodeCafe.Web.Infrastructure;

/// <summary>
/// Stable HTTP error response shape. Kept small and stable so clients can rely on it.
/// The shape intentionally avoids a full ProblemDetails style payload for now:
/// it adds noise without value for a workspace API.
/// </summary>
public sealed record ErrorResponse(string Code, string Message, string TraceId)
{
    /// <summary>
    /// Optional debug details. Only populated in development environments so
    /// we never leak stack traces or internal type names to production clients.
    /// </summary>
    public string? Details { get; init; }
}
