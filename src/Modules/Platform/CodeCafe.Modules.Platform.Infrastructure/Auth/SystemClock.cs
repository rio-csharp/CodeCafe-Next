using CodeCafe.Modules.Platform.Application.Abstractions;

namespace CodeCafe.Modules.Platform.Infrastructure.Auth;

/// <summary>
/// Default <see cref="IClock"/> implementation. The whole class is one line
/// of behaviour; we keep it on purpose so tests can substitute it.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
