namespace CodeCafe.Modules.Platform.Application.Abstractions;

/// <summary>
/// Clock abstraction used by handlers and entities that need the current
/// time. Lets tests and time-sensitive logic avoid using <c>DateTime.UtcNow</c>
/// directly.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
